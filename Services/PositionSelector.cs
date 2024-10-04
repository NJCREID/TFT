using TFT_API.Models.UserGuides;

namespace TFT_API.Services
{
    public class PositionSelector
    {
        private readonly Dictionary<string, string> Champions = new()
        {
            {"TFT11_Aatrox", "front_tank_ap"},
            {"TFT11_Ahri", "back_damage_ap"},
            {"TFT11_Alune", "back_damage_ap"},
            {"TFT11_Amumu", "front_tank_ap"},
            {"TFT11_Annie", "front_tank_ap"},
            {"TFT11_Aphelios", "back_damage_ad"},
            {"TFT11_Ashe", "back_damage_ad"},
            {"TFT11_Azir", "back_damage_ap"},
            {"TFT11_Bard", "back_damage_ap"},
            {"TFT11_Caitlyn", "back_damage_ad"},
            {"TFT11_ChoGath", "front_tank_ap"},
            {"TFT11_Darius", "front_damage_ad"},
            {"TFT11_Diana", "front_tank_ap"},
            {"TFT11_FortuneYord", "front_tank_ap"},
            {"TFT11_Galio", "front_tank_ap"},
            {"TFT11_Garen", "front_tank_ap"},
            {"TFT11_Gnar", "front_damage_ad"},
            {"TFT11_Hwei", "back_damage_ap"},
            {"TFT11_Illaoi", "front_tank_ap"},
            {"TFT11_Irelia", "back_damage_ad"},
            {"TFT11_Janna", "back_damage_ap"},
            {"TFT11_Jax", "front_damage_ad"},
            {"TFT11_Kaisa", "back_damage_ad"},
            {"TFT11_Kayle", "back_link_ap"},
            {"TFT11_Kayn", "second_lr_damage_ad"},
            {"TFT11_KhaZix", "front_damage_ad"},
            {"TFT11_Kindred", "back_damage_ap"},
            {"TFT11_KogMaw", "back_damage_ap"},
            {"TFT11_LeeSin", "second_lr_damage_ad"},
            {"TFT11_Lillia", "back_damage_ap"},
            {"TFT11_Lissandra", "second_damage_ap"},
            {"TFT11_Lux", "back_damage_ap"},
            {"TFT11_Malphite", "front_tank_ap"},
            {"TFT11_Morgana", "back_damage_ap"},
            {"TFT11_Nautilus", "front_tank_ap"},
            {"TFT11_Neeko", "front_tank_ap"},
            {"TFT11_Ornn", "front_tank_ap"},
            {"TFT11_Qiyana", "front_damage_ad"},
            {"TFT11_Rakan", "second_damage_ap"},
            {"TFT11_RekSai", "front_tank_ap"},
            {"TFT11_Riven", "front_tank_ad"},
            {"TFT11_Senna", "back_damage_ad"},
            {"TFT11_Sett", "front_link_ad"},
            {"TFT11_Shen", "front_tank_ap"},
            {"TFT11_Sivir", "back_damage_ad"},
            {"TFT11_Soraka", "back_damage_ap"},
            {"TFT11_Sylas", "front_damage_ap"},
            {"TFT11_Syndra", "back_damage_ap"},
            {"TFT11_TahmKench", "front_tank_ap"},
            {"TFT11_Teemo", "back_damage_ap"},
            {"TFT11_Thresh", "front_tank_ap"},
            {"TFT11_Tristana", "back_damage_ad"},
            {"TFT11_Udyr", "front_tank_ap"},
            {"TFT11_Volibear", "front_tank_ap"},
            {"TFT11_WuKong", "second_lr_damage_ad"},
            {"TFT11_Xayah", "back_damage_ad"},
            {"TFT11_Yasuo", "front_damage_ad"},
            {"TFT11_Yone", "second_lr_damage_ad"},
            {"TFT11_Yorick", "front_tank_ap"},
            {"TFT11_Zoe", "back_damage_ap"},
            {"TFT11_Zyra", "back_damage_ap"}
        };


        public List<Hex> CalculateUnitPositions(List<Hex> hexes)
        {
            Dictionary<int, string> occupiedHexes = [];
            hexes = [.. hexes.OrderBy(h => OrderUnits(h))
                .ThenByDescending(hex => hex.CurrentItems.Count)
                .ThenByDescending(hex => hex.Unit.Tier)
                .ThenByDescending(hex => hex.Unit.Health.ToList()[2])];

            int frontCount = hexes.Count(hex => Champions[hex.Unit.InGameKey].Contains("front"));
            foreach (var hex in hexes)
            {
                var unit = hex.Unit;
                var type = Champions[unit.InGameKey];

                switch (type)
                {
                    case "front_tank_ap":
                    case "front_tank_ad":
                    case "front_damage_ad":
                    case "front_damage_ap":
                        hex.Coordinates = FindAvailableFrontPosition(occupiedHexes, frontCount);
                        break;
                    case "back_damage_ap":
                    case "back_damage_ad":
                        hex.Coordinates = FindAvailableBackPosition(occupiedHexes);
                        break;
                    case "second_lr_damage_ad":
                        hex.Coordinates = FindAvailableSecondLRDamagePosition(occupiedHexes);
                        break;
                    case "second_damage_ap":
                    case "second_damage_ad":
                        hex.Coordinates = FindAvailableSecondDamagePosition(occupiedHexes);
                        break;
                    case "front_link_ad":
                        hex.Coordinates = FindAvailableFrontLinkPosition(occupiedHexes);
                        break;
                    case "back_link_ap":
                        hex.Coordinates = FindAvailableBackLinkPosition(occupiedHexes);
                        break;
                }

                occupiedHexes[hex.Coordinates] = Champions[unit.InGameKey];
            }

            return hexes;
        }

        public int OrderUnits(Hex hex)
        {
            string positionType = Champions[hex.Unit.InGameKey];
            if (positionType.Contains("link"))
                return 1;
            else if (positionType.Contains("second_lr"))
                return 2;
            else if (positionType.Contains("front_tank"))
                return 3;
            else if (positionType.Contains("front_damage"))
                return 3;
            else if (positionType.Contains("second_damage"))
                return 5;
            else if (positionType.Contains("back_damage"))
                return 6;
            else
                return int.MaxValue;
        }

        private static int FindAvailableFrontPosition(Dictionary<int, string> occupiedHexes, int frontCount)
        {
            if(frontCount == 1)
            {
                if (!occupiedHexes.ContainsKey(3)) return 3;
            }
            if(frontCount == 2)
            {
                if (!occupiedHexes.ContainsKey(1)) return 1;
                if (!occupiedHexes.ContainsKey(5)) return 5;
            }
            if (occupiedHexes.TryGetValue(2, out string? hex2) && hex2.Contains("front_link"))
            {
                if (!occupiedHexes.ContainsKey(1)) return 1;
                if (!occupiedHexes.ContainsKey(3)) return 3;
            }
            if (occupiedHexes.TryGetValue(4, out string? hex4) && hex4.Contains("front_link"))
            {
                if (!occupiedHexes.ContainsKey(5)) return 5;
                if (!occupiedHexes.ContainsKey(3)) return 3;
            }
            for (var i = 1; i < 6; i++)
            {
                if (!occupiedHexes.ContainsKey(i) && !occupiedHexes.ContainsKey(i - 1)) return i;
            }
            for (var i = 1; i < 6; i++)
            {
                if (!occupiedHexes.ContainsKey(i)) return i;
            }
            if ((!occupiedHexes.ContainsKey(0)) && !occupiedHexes.ContainsKey(7)) return 0;
            if ((!occupiedHexes.ContainsKey(6)) && !occupiedHexes.ContainsKey(13)) return 6;

            for (var i = 14; i < 21; i++)
            {
                if (!occupiedHexes.ContainsKey(i)) return i;
            }
            return -1;
        }

        private static int FindAvailableBackPosition(Dictionary<int, string> occupiedHexes)
        {
            if (occupiedHexes.TryGetValue(26, out string? hex26) && hex26.Contains("back_link"))
            {
                if (!occupiedHexes.ContainsKey(27)) return 27;
                if (!occupiedHexes.ContainsKey(25)) return 25;
            }
            if (occupiedHexes.TryGetValue(22, out string? hex22) && hex22.Contains("back_link"))
            {
                if (!occupiedHexes.ContainsKey(21)) return 21;
                if (!occupiedHexes.ContainsKey(23)) return 23;
            }
            if (!occupiedHexes.ContainsKey(21)) return 21;
            if (!occupiedHexes.ContainsKey(27)) return 27;
            for (int i = 23; i <= 25; i++)
            {
                if (!occupiedHexes.ContainsKey(i) && !occupiedHexes.ContainsKey(i - 1) && !occupiedHexes.ContainsKey(i +1)) return i;
            }
            if (!occupiedHexes.ContainsKey(24)) return 24;
            if (!occupiedHexes.ContainsKey(22)) return 22;
            if (!occupiedHexes.ContainsKey(26)) return 26;
            return -1; 
        }

        private static int FindAvailableSecondLRDamagePosition(Dictionary<int, string> occupiedHexes)
        {

            if (!occupiedHexes.ContainsKey(7))
                return 7;
            if(!occupiedHexes.ContainsKey(13))
                return 13;
            if (!occupiedHexes.ContainsKey(2))
                return 2;
            if (!occupiedHexes.ContainsKey(4))
                return 4;
            return -1;
        }

        private static int FindAvailableSecondDamagePosition(Dictionary<int, string> occupiedHexes)
        {
            for(int i = 8; i <= 12; i++)
            {
                if(occupiedHexes.ContainsKey(i - 6) || occupiedHexes.ContainsKey(i - 7))
                    return i;
            }
            return -1; 
        }

        private static int FindAvailableFrontLinkPosition(Dictionary<int, string> occupiedHexes)
        {
            if (!occupiedHexes.ContainsKey(2))
                return 2;
            if (!occupiedHexes.ContainsKey(4))
                return 4;
            return -1;
        }

        private static int FindAvailableBackLinkPosition(Dictionary<int, string> occupiedHexes)
        {
            if(!occupiedHexes.ContainsKey(22))
                return 22;
            if (!occupiedHexes.ContainsKey(26))
                return 26;
            return -1;
        }
    }
}
