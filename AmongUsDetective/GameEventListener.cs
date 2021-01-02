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
            int rand = Convert.ToUInt16(new Random().Next(0, 99)), count = 3, temp = rand, temp2 = rand - count;
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
                    await player.Character.SetSkinAsync((uint)(temp == 5 ? temp + 1 : temp) % 15);
                    await player.Character.SetHatAsync((uint)(temp2 == 82 ? temp2 + 1 : temp2) % 93);
                    await player.Character.SetColorAsync((byte)(temp % 11));
                    await player.Character.SetPetAsync((uint)temp2 % 10);
                } else {
                    _logger.LogInformation($"- {info.PlayerName} is a crewmate.");
                    await player.Character.SetSkinAsync((uint)(temp == 5 ? temp + 1 : temp) % 15);
                    await player.Character.SetHatAsync((uint)(temp2 == 82 ? temp2 + 1 : temp2) % 93);
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
        public async void OnPlayerStartMeeting(IPlayerStartMeetingEvent e) {
            _logger.LogInformation($"Player Started Meeting");
            IInnerPlayerControl bMurderer = null;
            if (e.Body != null) {
                bMurderer = _kills[e.Body];

                if (e.PlayerControl == _det) {
                    // Detective found body
                    switch (new Random().Next(1,4)) {
                        case 1:
                            await _det.SendChatToPlayerAsync($"Impostor was wearing {getSkin(bMurderer.PlayerInfo.SkinId)} on their body.");
                            break;
                        case 2:
                            await _det.SendChatToPlayerAsync($"Impostor was wearing {getHat(bMurderer.PlayerInfo.HatId)} on their head.");
                            break;
                        case 3:
                            await _det.SendChatToPlayerAsync($"Impostor was the color {getColor(bMurderer.PlayerInfo.ColorId)}.");
                            break;
                        case 4:
                            await _det.SendChatToPlayerAsync($"Impostor has {getPet(bMurderer.PlayerInfo.PetId)} for a pet.");
                            break;
                        default:
                            await _det.SendChatToPlayerAsync($"Impostor was unable to be detected");
                            break;
                    }
                }
            }
        }

        private string getSkin(uint id) {
            switch (id) {
                case (uint)SkinType.Archae:
                    return "Archae";
                case (uint)SkinType.Astro:
                    return "Astro";
                case (uint)SkinType.Capt:
                    return "Capt";
                case (uint)SkinType.Hazmat:
                    return "Hazmat";
                case (uint)SkinType.Mech:
                    return "Mech";
                case (uint)SkinType.Military:
                    return "Military";
                case (uint)SkinType.Miner:
                    return "Miner";
                case (uint)SkinType.None:
                    return "Nothing";
                case (uint)SkinType.Police:
                    return "Police";
                case (uint)SkinType.Science:
                    return "Science";
                case (uint)SkinType.Security:
                    return "Security";
                case (uint)SkinType.SuitB:
                    return "SuitB";
                case (uint)SkinType.SuitW:
                    return "SuitW";
                case (uint)SkinType.Tarmac:
                    return "Tarmac";
                case (uint)SkinType.Wall:
                    return "Wall";
                case (uint)SkinType.Winter:
                    return "Winter";
                default:
                    return "Unknown";
            }
        }

        private string getHat(uint id) {
            switch (id) {
                case (uint)HatType.Antenna:
                    return "Antenna";
                case (uint)HatType.Archae:
                    return "Archae (Cowboy looking hat)";
                case (uint)HatType.Astronaut:
                    return "Astronaut";
                case (uint)HatType.Balloon:
                    return "Balloon";
                case (uint)HatType.Banana:
                    return "Banana";
                case (uint)HatType.BaseballCap:
                    return "Baseball Cap";
                case (uint)HatType.BatEyes:
                    return "Bat Eyes";
                case (uint)HatType.BatWings:
                    return "Bat Wings";
                case (uint)HatType.Beanie:
                    return "Beanie";
                case (uint)HatType.Bear:
                    return "Bear";
                case (uint)HatType.BirdNest:
                    return "Bird Nest";
                case (uint)HatType.BlackBelt:
                    return "Black Belt";
                case (uint)HatType.BrainSlug:
                    return "Brain Slug";
                case (uint)HatType.BushHat:
                    return "Bush Hat";
                case (uint)HatType.Candycanes:
                    return "Candycanes";
                case (uint)HatType.CaptainsHat:
                    return "Captains Hat";
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
                    return "Crown";
                case (uint)HatType.DoRag:
                    return "Do-Rag";
                case (uint)HatType.DoubleTopHat:
                    return "Double Top Hat";
                case (uint)HatType.DumSticker:
                    return "Dum Sticker";
                case (uint)HatType.Egg:
                    return "Egg";
                case (uint)HatType.ElfHat:
                    return "ElfHat";
                case (uint)HatType.Eyebrows:
                    return "Eyebrows";
                case (uint)HatType.Fedora:
                    return "Fedora";
                case (uint)HatType.Fedora2:
                    return "Fedora";
                case (uint)HatType.Fez:
                    return "Fez";
                case (uint)HatType.Flamingo:
                    return "Flamingo";
                case (uint)HatType.FlowerPin:
                    return "Flower Pin";
                case (uint)HatType.Flowerpot:
                    return "Flowerpot";
                case (uint)HatType.Fred:
                    return "Fred";
                case (uint)HatType.GeneralHat:
                    return "General Hat";
                case (uint)HatType.Goggles:
                    return "Goggles";
                case (uint)HatType.Goggles2:
                    return "Goggles";
                case (uint)HatType.GreyThing:
                    return "Grey Thing";
                case (uint)HatType.HaloHat:
                    return "Halo Hat";
                case (uint)HatType.HardHat:
                    return "Hard Hat";
                case (uint)HatType.Headphones:
                    return "Headphones";
                case (uint)HatType.Helmet:
                    return "Helmet";
                case (uint)HatType.HeroCap:
                    return "Hero Cap";
                case (uint)HatType.Horns:
                    return "Horns";
                case (uint)HatType.HunterCap:
                    return "Hunter Cap";
                case (uint)HatType.JungleHat:
                    return "Jungle Hat";
                case (uint)HatType.Lights:
                    return "Lights";
                case (uint)HatType.Machete:
                    return "Machete";
                case (uint)HatType.MaskHat:
                    return "Mask Hat";
                case (uint)HatType.Military:
                    return "Military";
                case (uint)HatType.MinerCap:
                    return "Miner Cap";
                case (uint)HatType.MiniCrewmate:
                    return "Mini Crewmate";
                case (uint)HatType.Mohawk:
                    return "Mohawk";
                case (uint)HatType.NewYears2018:
                    return "New Years 2018";
                case (uint)HatType.NinjaMask:
                    return "Ninja Mask";
                case (uint)HatType.NoHat:
                    return "No Hat";
                case (uint)HatType.PaperHat:
                    return "Paper Hat";
                case (uint)HatType.PaperMask:
                    return "Paper Mask";
                case (uint)HatType.PartyHat:
                    return "Party Hat";
                case (uint)HatType.PipCap:
                    return "Pip Cap";
                case (uint)HatType.Pirate:
                    return "Pirate";
                case (uint)HatType.Plague:
                    return "Plague";
                case (uint)HatType.Plant:
                    return "Plant";
                case (uint)HatType.PlungerHat:
                    return "Plunger Hat";
                case (uint)HatType.Police:
                    return "Police Hat";
                case (uint)HatType.Present:
                    return "Present";
                case (uint)HatType.Pumpkin:
                    return "Pumpkin";
                case (uint)HatType.RamHorns:
                    return "Ram Horns";
                case (uint)HatType.Reindeer:
                    return "Reindeer";
                case (uint)HatType.Santa:
                    return "Santa";
                case (uint)HatType.ScaryBag:
                    return "Scary Bag";
                case (uint)HatType.ScubaHat:
                    return "Scuba Hat";
                case (uint)HatType.Security:
                    return "Security";
                case (uint)HatType.Snowman:
                    return "Snowman";
                case (uint)HatType.Snowman2:
                    return "Snowman";
                case (uint)HatType.Stethescope:
                    return "Stethescope";
                case (uint)HatType.StickminHat:
                    return "Stickmin Hat";
                case (uint)HatType.StrapHat:
                    return "StrapHat";
                case (uint)HatType.StrawHat:
                    return "Straw Hat";
                case (uint)HatType.TenGallonHat:
                    return "Ten Gallon Hat";
                case (uint)HatType.ThirdEyeHat:
                    return "Third Eye Hat";
                case (uint)HatType.ToiletPaperHat:
                    return "Toilet Paper Hat";
                case (uint)HatType.TopHat:
                    return "Top Hat";
                case (uint)HatType.Toppat:
                    return "Toppat";
                case (uint)HatType.TowelWizard:
                    return "Towel Wizard";
                case (uint)HatType.Tree:
                    return "Tree";
                case (uint)HatType.Ushanka:
                    return "Ushanka";
                case (uint)HatType.Viking:
                    return "Viking";
                case (uint)HatType.WallCap:
                    return "Wall Cap";
                case (uint)HatType.WhiteHat:
                    return "White Hat";
                case (uint)HatType.WinterHat:
                    return "Winter Hat";
                case (uint)HatType.Witch:
                    return "Witch";
                case (uint)HatType.Wolf:
                    return "Wolf";
                default:
                    return "Unknown";
            }
        }

        private string getColor(byte id) {
            switch (id) {
                case (byte)ColorType.Black:
                    return "Black";
                case (byte)ColorType.Blue:
                    return "Blue";
                case (byte)ColorType.Brown:
                    return "Brown";
                case (byte)ColorType.Cyan:
                    return "Cyan";
                case (byte)ColorType.Green:
                    return "Green";
                case (byte)ColorType.Lime:
                    return "Lime";
                case (byte)ColorType.Orange:
                    return "Orange";
                case (byte)ColorType.Pink:
                    return "Pink";
                case (byte)ColorType.Purple:
                    return "Purple";
                case (byte)ColorType.Red:
                    return "Red";
                case (byte)ColorType.White:
                    return "White";
                case (byte)ColorType.Yellow:
                    return "Yellow";
                default:
                    return "Unknown";
            }
        }

        private string getPet(uint id) {
            switch (id) {
                case (uint)PetType.Alien:
                    return "Alien";
                case (uint)PetType.Bedcrab:
                    return "Bedcrab";
                case (uint)PetType.Crewmate:
                    return "Crewmate";
                case (uint)PetType.Doggy:
                    return "Doggy";
                case (uint)PetType.Ellie:
                    return "Ellie";
                case (uint)PetType.Hamster:
                    return "Hamster";
                case (uint)PetType.NoPet:
                    return "NoPet";
                case (uint)PetType.Robot:
                    return "Robot";
                case (uint)PetType.Squig:
                    return "Squig";
                case (uint)PetType.Stickmin:
                    return "Stickmin";
                case (uint)PetType.Ufo:
                    return "Ufo";
                default:
                    return "Unknown";
            }
        }

        [EventListener]
        public void OnGameEnd(IGameEndedEvent e) {
            _logger.LogInformation($"Game has ended.");
        }
    }
}
