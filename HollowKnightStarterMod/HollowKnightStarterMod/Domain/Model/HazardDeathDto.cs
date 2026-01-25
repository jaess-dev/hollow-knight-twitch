namespace HollowKnightStarterMod.Domain.Model
{
    public class HazardDeathDto
    {
        public HazardTypeDto HazardTypeDto { get; }
        public HazardDeathDto(HazardTypeDto hazardTypeDto)
        {
            HazardTypeDto = hazardTypeDto;
        }
    }

    public enum HazardTypeDto
    {
        NON_HAZARD,
        SPIKES,
        ACID,
        LAVA,
        PIT
    }

    public static class HazardTypeDtoMethods
    {
        public static HazardTypeDto? FromString(string str) => str switch
        {
            "Non Hazard" => HazardTypeDto.NON_HAZARD,
            "Spikes" => HazardTypeDto.SPIKES,
            "Acid" => HazardTypeDto.ACID,
            "Lava" => HazardTypeDto.LAVA,
            "Pit" => HazardTypeDto.PIT,
            _ => null
        };

        public static string ToString(this HazardTypeDto self) => self switch
        {
            HazardTypeDto.NON_HAZARD => "Non Hazard",
            HazardTypeDto.SPIKES => "Spikes",
            HazardTypeDto.ACID => "Acid",
            HazardTypeDto.LAVA => "Lava",
            HazardTypeDto.PIT => "Pit",
            _ => ""
        };
    }
}