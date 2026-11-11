namespace MachinesDataAnalyzingSystem.Shared.DTO.MachineDatas;

public class MachineDataDTO
{
    /// <summary>
    /// Speed in Meters per second.
    /// </summary>
    public float AverageMotorsSpeed { get; set; }
    /// <summary>
    /// Temperature in celcues.
    /// </summary>
    public float AverageTemperature { get; set; }
    public DateTime DateTime { get; set; }
    public int NumberOfProcessedResources { get; set; }
    public float NumberOfSecondsToProcessResources { get; set; }
    public Guid MachineId { get; set; }
}