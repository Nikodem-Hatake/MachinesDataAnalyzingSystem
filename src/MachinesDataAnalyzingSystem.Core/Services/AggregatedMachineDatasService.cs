using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Core.Domain.RepositoryContracts;
using MachinesDataAnalyzingSystem.Core.ServiceContracts;
using MachinesDataAnalyzingSystem.Shared;
using MachinesDataAnalyzingSystem.Shared.DTO.AggregatedMachineDatas;
using MachinesDataAnalyzingSystem.Shared.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MachinesDataAnalyzingSystem.Core.Services;

public class AggregatedMachineDatasService(IAggregatedMachineDatasRepository aggregatedMachineDatasRepository,
    IMachineDatasRepository machineDatasRepository, IDistributedCache distributedCache, 
    ILogger<AggregatedMachineDatasService> logger) : IAggregatedMachineDatasService
{
    private async Task<AggregatedMachineData?> Aggregate(Guid machineId, DateTime startDate, HowLongIsAggregatedMachineData howLong)
    {
        var machineDatas = await GetMachineDatasForAggregation(machineId, startDate, howLong);
        if(machineDatas.Count == 0)
        {
            return null;
        }

        return new()
        {
            AverageMotorsSpeed = machineDatas.Sum(x => x.AverageMotorsSpeed) / machineDatas.Count,
            AverageTemperature = machineDatas.Sum(x => x.AverageTemperature) / machineDatas.Count,
            HowLongIsAggregatedMachineData = howLong,
            Id = Guid.NewGuid(),
            MachineId = machineId,
            MaxTemperature = machineDatas.Max(x => x.AverageTemperature),
            MinTemperature = machineDatas.Min(x => x.AverageTemperature),
            StartDate = startDate,
            TotalNumberOfProcessedResources = machineDatas.Sum(x => x.NumberOfProcessedResources),
            TotalNumberOfSecondsToProcessResources = machineDatas.Sum(x => x.NumberOfSecondsToProcessResources)
        };
    }

    private void ClearDateTimeUnnecessaryValues(ref DateTime dateTime, HowLongIsAggregatedMachineData howLong)
    {
        dateTime = DateTime.ParseExact(dateTime.ToString(GlobalConstants.AGGREGATED_DATA_DATE_TIME_FORMAT), 
            GlobalConstants.AGGREGATED_DATA_DATE_TIME_FORMAT, null);
        switch(howLong)
        {
            case HowLongIsAggregatedMachineData.TenMinutes:
            {
                dateTime = dateTime.AddMinutes(-(dateTime.Minute % 10));
                break;
            }
            case HowLongIsAggregatedMachineData.OneHour:
            {
                dateTime = dateTime.AddMinutes(-dateTime.Minute);
                break;
            }
            case HowLongIsAggregatedMachineData.OneDay:
            {
                dateTime = dateTime = dateTime.Date;
                break;
            }
            case HowLongIsAggregatedMachineData.OneWeek:
            {
                dateTime = dateTime.Date.AddDays(-((int)dateTime.DayOfWeek) + 1);
                break;
            }
            case HowLongIsAggregatedMachineData.OneMonth:
            {
                dateTime = dateTime.Date.AddDays(-dateTime.Day).AddDays(1);
                break;
            }
        }
    }
    
    private async Task<AggregatedMachineDataDTO?> GetAggregatedMachineData(Guid machineId, DateTime startDate,
        HowLongIsAggregatedMachineData howLong)
    {
        var cacheKey = $"{machineId}/{startDate.ToString(GlobalConstants.AGGREGATED_DATA_DATE_TIME_FORMAT)}/{((int)howLong)}";
        var aggregatedMachineDataFromCache = await distributedCache.GetStringAsync(cacheKey);
        if(aggregatedMachineDataFromCache is not null)
        {
            logger.LogInformation($"Cache hit for {cacheKey}");
            return JsonSerializer.Deserialize<AggregatedMachineDataDTO>(aggregatedMachineDataFromCache, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
        }

        logger.LogInformation($"Cache miss for {cacheKey}");
        var aggregatedMachineDataFromDatabase = await aggregatedMachineDatasRepository
            .GetAggregatedMachineData(machineId, startDate, howLong);
        if(aggregatedMachineDataFromDatabase is not null)
        {
            var aggregatedMachineDataDTO = aggregatedMachineDataFromDatabase.ToDTO();
            await SetValueInCache(cacheKey, JsonSerializer.Serialize(aggregatedMachineDataDTO));
            return aggregatedMachineDataDTO;
        }

        var aggregatedMachineData = await Aggregate(machineId, startDate, howLong);
        if(aggregatedMachineData is null)
        {
            return null;
        }
        var aggregatedMachineDataAsDTO = aggregatedMachineData.ToDTO();
        await aggregatedMachineDatasRepository.AddAggregatedMachineData(aggregatedMachineData);
        await SetValueInCache(cacheKey, JsonSerializer.Serialize(aggregatedMachineDataAsDTO));
        return aggregatedMachineDataAsDTO;
    }

    public async Task<List<AggregatedMachineDataDTO>> GetAggregatedMachineDatas(Guid MachineId, 
        DateTime startDate, HowLongIsAggregatedMachineData howLong, int howMany)
    {
        ClearDateTimeUnnecessaryValues(ref startDate, howLong);
        if(!IsDateTimeCorrectForAggregation(startDate, howLong, howMany))
        {
            return [];
        }

        List<AggregatedMachineDataDTO> response = [];
        for(int i = 0; i < howMany; ++i)
        {
            var aggregatedMachineData = await GetAggregatedMachineData(MachineId, startDate.AddMinutes(10 * i), howLong);
            if(aggregatedMachineData is not null)
            {
                response.Add(aggregatedMachineData);
            }
        }
        return response;
    }

    private async Task<List<MachineData>> GetMachineDatasForAggregation(Guid machineId,
        DateTime startDate, HowLongIsAggregatedMachineData howLong)
    {
        switch(howLong)
        {
            case HowLongIsAggregatedMachineData.TenMinutes:
            {
                return await machineDatasRepository.GetMachineDatasForAggregation(machineId, startDate,
                    startDate.AddMinutes(10));
            }
            case HowLongIsAggregatedMachineData.OneHour:
            {
                return await machineDatasRepository.GetMachineDatasForAggregation(machineId, startDate,
                    startDate.AddHours(1));
            }
            case HowLongIsAggregatedMachineData.OneDay:
            {
                return await machineDatasRepository.GetMachineDatasForAggregation(machineId, startDate,
                    startDate.AddDays(1));
            }
            case HowLongIsAggregatedMachineData.OneWeek:
            {
                return await machineDatasRepository.GetMachineDatasForAggregation(machineId, startDate,
                    startDate.AddDays(7));
            }
            case HowLongIsAggregatedMachineData.OneMonth:
            {
                return await machineDatasRepository.GetMachineDatasForAggregation(machineId, startDate,
                    startDate.AddMonths(1));
            }
        }
        return [];
    }
    
    private bool IsDateTimeCorrectForAggregation(DateTime dateTime, HowLongIsAggregatedMachineData howLong, int howMany)
    {
        switch(howLong)
        {
            case HowLongIsAggregatedMachineData.TenMinutes: 
            {
                return dateTime.AddMinutes(howMany * 10) <= DateTime.UtcNow;
            }
            case HowLongIsAggregatedMachineData.OneHour: 
            {
                return dateTime.AddHours(howMany) <= DateTime.UtcNow;
            }
            case HowLongIsAggregatedMachineData.OneDay:
            {
                return dateTime.AddDays(howMany) <= DateTime.UtcNow;
            }
            case HowLongIsAggregatedMachineData.OneWeek:
            {
                return dateTime.AddDays(howMany * 7) <= DateTime.UtcNow;
            }
            case HowLongIsAggregatedMachineData.OneMonth:
            {
                return dateTime.AddMonths(howMany) <= DateTime.UtcNow;
            }
        }
        return false;
    }

    private async Task SetValueInCache(string key, string value)
    {
        await distributedCache.SetStringAsync(key, value, new DistributedCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromHours(24)
        });
    }
}