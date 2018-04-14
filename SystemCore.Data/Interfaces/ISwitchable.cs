using SystemCore.Data.Enums;

namespace SystemCore.Data.Interfaces
{
    public interface ISwitchable
    {
        Status Status { get; set; }
    }
}