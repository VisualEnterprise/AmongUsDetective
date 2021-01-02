using Impostor.Api.Events;
using Impostor.Api.Events.Meeting;
using Impostor.Api.Events.Player;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Innersloth.Customization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace cc.ts13.AmongUsDetective.Handlers {
    class GameEventListener : IEventListener {

        private readonly ILogger<Detective> _logger;
        private Dictionary<IInnerPlayerControl, IInnerPlayerControl> _kills;

        private IInnerPlayerControl _det;

        public GameEventListener(ILogger<Detective> logger) {
            _logger = logger;
            _kills = new Dictionary<IInnerPlayerControl, IInnerPlayerControl>();
        }

        [EventListener]
        public async void OnGameStarted(IGameStartedEvent e) {
            _logger.LogInformation($"Game is starting.");
            int num = new Random().Next(1, e.Game.PlayerCount) - 1;
            bool det = true;
            int rand = Convert.ToUInt16(new Random().Next(0, 99)), count = 5, temp = rand, temp2 = rand - count;
            bool test = true;
            foreach (var player in e.Game.Players) {
                
                var info = player.Character.PlayerInfo;
                var isImpostor = info.IsImpostor;
                if (!isImpostor && num-- <= 0 && det) {
                    det = false;
                    _det = player.Character;
                    _logger.LogInformation($"- {info.PlayerName} is detective");
                    await player.Character.SetSkinAsync(SkinType.Police);
                    await player.Character.SetHatAsync(HatType.CopHat);
                } else if (isImpostor) {
                    _logger.LogInformation($"- {info.PlayerName} is an impostor.");
                    await player.Character.SetSkinAsync((uint)temp % 15);
                    await player.Character.SetHatAsync((uint)temp2 % 93);
                    await player.Character.SetColorAsync((byte)(temp % 11));
                    await player.Character.SetPetAsync((uint)temp2 % 10);
                } else {
                    _logger.LogInformation($"- {info.PlayerName} is a crewmate.");
                    await player.Character.SetSkinAsync((uint)temp % 15);
                    await player.Character.SetHatAsync((uint)temp2 % 93);
                    await player.Character.SetColorAsync((byte)(temp % 11));
                    await player.Character.SetPetAsync((uint)temp2 % 10);
                }
                if (count-- <= 0) {
                    count = 5;
                    temp = rand;
                    test = !test;
                } else {
                    temp--;
                    if (test)
                        temp2++;
                    else
                        temp2--;
                }
            }
        }

        [EventListener]
        public void OnPlayerMurder(IPlayerMurderEvent e) {
            _logger.LogInformation($"Player murdered");
            _kills.Add(e.Victim, e.PlayerControl);
        }

        [EventListener]
        public void OnPlayerStartMeeting(IPlayerStartMeetingEvent e) {
            _logger.LogInformation($"Player Started Meeting");
            IInnerPlayerControl bMurderer = null;
            if (e.Body != null) {
                bMurderer = _kills[e.Body];

                if (e.PlayerControl == _det) {
                    // Detective found body
                    switch (new Random().Next(1,4)) {
                        case 1:
                            _det.SendChatToPlayerAsync($"Murderer was wearing {getSkin(bMurderer.PlayerInfo.SkinId)} suit.");
                            break;
                        case 2:
                            _det.SendChatToPlayerAsync($"Murderer was wearing {bMurderer.PlayerInfo.HatId} hat.");
                            break;
                        case 3:
                            _det.SendChatToPlayerAsync($"Murderer was the color {bMurderer.PlayerInfo.ColorId}.");
                            break;
                        case 4:
                            _det.SendChatToPlayerAsync($"Murderer has {bMurderer.PlayerInfo.PetId} for a pet.");
                            break;
                        default:
                            _det.SendChatToPlayerAsync($"Murderer was unable to be detected");
                            break;
                    }
                }
            }
        }

        private string getSkin(uint id) {
            switch (id) {
                case (uint)SkinType.Archae:
                    return "ARCHAE";
                case (uint)SkinType.Astro:
                    return "ASTRO";
                case (uint)SkinType.Capt:
                    return "CAPT";
                case (uint)SkinType.Hazmat:
                    return "HAZMAT";
                case (uint)SkinType.Mech:
                    return "MECH";
                case (uint)SkinType.Military:
                    return "MILITARY";
                case (uint)SkinType.Miner:
                    return "MINER";
                case (uint)SkinType.None:
                    return "NOTHING";
                case (uint)SkinType.Police:
                    return "POLICE";
                case (uint)SkinType.Science:
                    return "SCIENTIST";
                case (uint)SkinType.Security:
                    return "SECURITY";
                case (uint)SkinType.SuitB:
                    return "SUITB";
                case (uint)SkinType.SuitW:
                    return "SUITW";
                case (uint)SkinType.Tarmac:
                    return "TARMAC";
                case (uint)SkinType.Wall:
                    return "WALL";
                case (uint)SkinType.Winter:
                    return "WINTER";
                default:
                    return "Unknown";
            }
        }

        private string getHat(uint id) {
            switch (id) {
                case (uint)HatType.Antenna:
                    return "Antenna";
                case (uint)HatType.Archae:
                    return "Archae";
                case (uint)HatType.Astronaut:
                    return "Astronaut";
                case (uint)HatType.Balloon:
                    return "Balloon";
                case (uint)HatType.Banana:
                    return "Banana";
                case (uint)HatType.BaseballCap:
                    return "BaseballCap";
                case (uint)HatType.BatEyes:
                    return "BatEyes";
                case (uint)HatType.BatWings:
                    return "BatWings";
                case (uint)HatType.Beanie:
                    return "Beanie";
                case (uint)HatType.Bear:
                    return "Bear";
                case (uint)HatType.BirdNest:
                    return "BirdNest";
                case (uint)HatType.BlackBelt:
                    return "BlackBelt";
                case (uint)HatType.BrainSlug:
                    return "BrainSlug";
                case (uint)HatType.BushHat:
                    return "BushHat";
                case (uint)HatType.Candycanes:
                    return "Candycanes";
                case (uint)HatType.CaptainsHat:
                    return "CaptainsHat";
                case (uint)HatType.Caution:
                    return "Caution";
                case (uint)HatType.Cheese:
                    return "Cheese";
                case (uint)HatType.Chef:
                    return "Chef";
                case (uint)HatType.Cherry:
                    return "Cherry";
                case (uint)HatType.CopHat:
                    return "CopHat";
                case (uint)HatType.Crown:
                    return "";
                case (uint)HatType.DoRag:
                    return "Crown";
                case (uint)HatType.DoubleTopHat:
                    return "DoubleTopHat";
                case (uint)HatType.DumSticker:
                    return "DumSticker";
                case (uint)HatType.Egg:
                    return "Egg";
                case (uint)HatType.ElfHat:
                    return "ElfHat";
                case (uint)HatType.Eyebrows:
                    return "Eyebrows";
                case (uint)HatType.Fedora:
                    return "Fedora";
                case (uint)HatType.Fedora2:
                    return "Fedora2";
                case (uint)HatType.Fez:
                    return "Fez";
                case (uint)HatType.Flamingo:
                    return "Flamingo";
                case (uint)HatType.FlowerPin:
                    return "FlowerPin";
                case (uint)HatType.Flowerpot:
                    return "Flowerpot";
                case (uint)HatType.Fred:
                    return "Fred";
                case (uint)HatType.GeneralHat:
                    return "GeneralHat";
                case (uint)HatType.Goggles:
                    return "Goggles";
                case (uint)HatType.Goggles2:
                    return "Goggles2";
                case (uint)HatType.GreyThing:
                    return "GreyThing";
                case (uint)HatType.HaloHat:
                    return "HaloHat";
                case (uint)HatType.HardHat:
                    return "HardHat";
                case (uint)HatType.Headphones:
                    return "Headphones";
                case (uint)HatType.Helmet:
                    return "Helmet";
                case (uint)HatType.HeroCap:
                    return "HeroCap";
                case (uint)HatType.Horns:
                    return "Horns";
                case (uint)HatType.HunterCap:
                    return "HunterCap";
                case (uint)HatType.JungleHat:
                    return "JungleHat";
                case (uint)HatType.Lights:
                    return "Lights";
                case (uint)HatType.Machete:
                    return "Machete";
                case (uint)HatType.MaskHat:
                    return "";
                case (uint)HatType.Military:
                    return "";
                case (uint)HatType.MinerCap:
                    return "";
                case (uint)HatType.MiniCrewmate:
                    return "";
                case (uint)HatType.Mohawk:
                    return "";
                case (uint)HatType.NewYears2018:
                    return "";
                case (uint)HatType.NinjaMask:
                    return "";
                case (uint)HatType.NoHat:
                    return "";
                case (uint)HatType.PaperHat:
                    return "";
                case (uint)HatType.PaperMask:
                    return "";
                case (uint)HatType.PartyHat:
                    return "";
                case (uint)HatType.PipCap:
                    return "";
                case (uint)HatType.Pirate:
                    return "";
                case (uint)HatType.Plague:
                    return "";
                case (uint)HatType.Plant:
                    return "";
                case (uint)HatType.PlungerHat:
                    return "";
                case (uint)HatType.Police:
                    return "";
                case (uint)HatType.Present:
                    return "";
                case (uint)HatType.Pumpkin:
                    return "";
                case (uint)HatType.RamHorns:
                    return "";
                case (uint)HatType.Reindeer:
                    return "";
                case (uint)HatType.Santa:
                    return "";
                case (uint)HatType.ScaryBag:
                    return "";
                case (uint)HatType.ScubaHat:
                    return "";
                case (uint)HatType.Security:
                    return "";
                case (uint)HatType.Snowman:
                    return "";
                case (uint)HatType.Snowman2:
                    return "";
                case (uint)HatType.Stethescope:
                    return "";
                case (uint)HatType.StickminHat:
                    return "";
                case (uint)HatType.StrapHat:
                    return "";
                case (uint)HatType.StrawHat:
                    return "";
                case (uint)HatType.TenGallonHat:
                    return "";
                case (uint)HatType.ThirdEyeHat:
                    return "";
                case (uint)HatType.ToiletPaperHat:
                    return "";
                case (uint)HatType.TopHat:
                    return "";
                case (uint)HatType.Toppat:
                    return "";
                case (uint)HatType.TowelWizard:
                    return "";
                case (uint)HatType.Tree:
                    return "";
                case (uint)HatType.Ushanka:
                    return "";
                case (uint)HatType.Viking:
                    return "";
                case (uint)HatType.WallCap:
                    return "";
                case (uint)HatType.WhiteHat:
                    return "";
                case (uint)HatType.WinterHat:
                    return "";
                case (uint)HatType.Witch:
                    return "";
                case (uint)HatType.Wolf:
                    return "";
                default:
                    return"";
            }
        }

        [EventListener]
        public void OnGameEnd(IGameEndedEvent e) {
            _logger.LogInformation($"Game has ended.");
        }
    }
}
