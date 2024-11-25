using System.Collections.Generic;

namespace pms_backend.Models;

public class ReporteesResponse
{
    public IEnumerable<UserDto> CurrentReportees { get; set; } = new List<UserDto>();
    public IEnumerable<UserDto> PreviousReportees { get; set; } = new List<UserDto>();
}
