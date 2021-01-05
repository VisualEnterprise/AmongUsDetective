using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Innersloth.Customization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;

namespace org.visualenterprise.AmongUsDetective.Handlers {
    class GameEventListener : IEventListener {

        private readonly ILogger<Detective> _logger;

        private struct DetectiveGame {
            public Dictionary<IInnerPlayerControl, IInnerPlayerControl> Kills { get; set; }
            public int DetectiveClientId { get; set; }
            public bool Enabled { get; set; }
            public bool RandomDetectiveOutfit { get; set; }
        }

        private Dictionary<string, DetectiveGame> _games;

        public GameEventListener(ILogger<Detective> logger) {
            _logger = logger;
            _games = new Dictionary<string, DetectiveGame>();
        }

        [EventListener]
        public void OnGameCreated(IGameCreatedEvent e) {
            DetectiveGame game = new DetectiveGame {
                Kills = new Dictionary<IInnerPlayerControl, IInnerPlayerControl>(),
                DetectiveClientId = -1,
                Enabled = true,
                RandomDetectiveOutfit = false
            };
            _games.Add(e.Game.Code, game);
        }

        [EventListener]
        public void OnPlayerChat(IPlayerChatEvent e) {
            _logger.LogInformation($"{e.PlayerControl.PlayerInfo.PlayerName} said {e.Message}");
            if (e.Game.GameState == GameStates.NotStarted && e.Message.StartsWith("/")) {
                Task.Run(async () => await RunCommands(e).ConfigureAwait(false));
            }
        }

        private async Task RunCommands(IPlayerChatEvent e) {
            DetectiveGame game = _games[e.Game.Code];
            switch (e.Message.ToLowerInvariant()) {
                case "/detective on":
                    if (e.ClientPlayer.IsHost) {
                        game.Enabled = true;

                        await e.PlayerControl.SendChatAsync("The Detective mod is now on!").ConfigureAwait(false);
                    } else {
                        await e.PlayerControl.SendChatAsync("You need to be host to change roles.").ConfigureAwait(false);
                    }
                    break;
                case "/detective off":
                    if (e.ClientPlayer.IsHost) {
                        game.Enabled = false;

                        await e.PlayerControl.SendChatAsync("The Detective mod is now off!").ConfigureAwait(false);
                    } else {
                        await e.PlayerControl.SendChatAsync("You need to be host to change roles.").ConfigureAwait(false);
                    }
                    break;
                case "/detective ro":
                case "/detective randomoutfit":
                    if (e.ClientPlayer.IsHost) {
                        game.RandomDetectiveOutfit = true;

                        await e.PlayerControl.SendChatAsync("The Detective now wears a random outfit!").ConfigureAwait(false);
                        await e.PlayerControl.SendChatAsync("You must check chat window to see if Detective!").ConfigureAwait(false);
                    } else {
                        await e.PlayerControl.SendChatAsync("You need to be host to change roles.").ConfigureAwait(false);
                    }
                    break;
                case "/detective oo":
                case "/detective officeroutfit":
                    if (e.ClientPlayer.IsHost) {
                        game.RandomDetectiveOutfit = false;

                        await e.PlayerControl.SendChatAsync("The Detective now wears the office outfit!").ConfigureAwait(false);
                    } else {
                        await e.PlayerControl.SendChatAsync("You need to be host to change roles.").ConfigureAwait(false);
                    }
                    break;
                case "/detective help":
                    await e.PlayerControl.SendChatAsync("When the special Detective mod is on, one of the cremate(s) are able to get hints about the impostor").ConfigureAwait(false);
                    await e.PlayerControl.SendChatAsync("If and only if the detective reports the body do they get a hint as to who the impostor is that committed the crime").ConfigureAwait(false);
                    await e.PlayerControl.SendChatAsync("The clues can include color, outfit, hat, pet. The clues can also be repeated").ConfigureAwait(false);
                    await e.PlayerControl.SendChatAsync("The host can turn the Detective mod on and off by typing '/detective on' or '/detective off'.").ConfigureAwait(false);
                    await e.PlayerControl.SendChatAsync("The host can also change the detective to a random outfit with '/detective randomOutfit' or officer with '/detective officerOutfit'.").ConfigureAwait(false);
                    break;
            }
            _games[e.Game.Code] = game;
        }

        [EventListener]
        public void OnSetStartCounter(IPlayerSetStartCounterEvent e) {
            if (e.SecondsLeft == 5) {
                DetectiveGame game = _games[e.Game.Code];
                int num = new Random().Next(1, e.Game.PlayerCount) - 1;
                bool det = true;
                int rand = Convert.ToUInt16(new Random().Next(0, 99)), count = 3, temp = rand, temp2 = rand - count;
                bool test = true;
                foreach (var player in e.Game.Players) {
                    var info = player.Character.PlayerInfo;
                    var isImpostor = info.IsImpostor;
                    if (!isImpostor && num-- <= 0 && det) {
                        det = false;
                        game.DetectiveClientId = player.Client.Id;
                        _logger.LogInformation($"- {info.PlayerName} is detective");
                        if (game.RandomDetectiveOutfit) {
                            Task.Run(async () => await player.Character.SetSkinAsync((uint)(temp % 15 == 5 ? temp + 1 : temp) % 15));
                            Task.Run(async () => await player.Character.SetHatAsync((uint)(temp2 % 93 == 82 ? temp2 + 1 : temp2) % 93));
                        } else {
                            Task.Run(async () => await player.Character.SetSkinAsync(SkinType.Police));
                            Task.Run(async () => await player.Character.SetHatAsync(HatType.CopHat));
                        }
                        Task.Run(async () => await player.Character.SetColorAsync((byte)(temp % 11)));
                        Task.Run(async () => await player.Character.SetPetAsync((uint)temp2 % 10));
                    } else if (isImpostor) {
                        _logger.LogInformation($"- {info.PlayerName} is an impostor.");
                        Task.Run(async () => await player.Character.SetSkinAsync((uint)(temp % 15 == 5 ? temp + 1 : temp) % 15));
                        Task.Run(async () => await player.Character.SetHatAsync((uint)(temp2 % 93 == 82 ? temp2 + 1 : temp2) % 93));
                        Task.Run(async () => await player.Character.SetColorAsync((byte)(temp % 11)));
                        Task.Run(async () => await player.Character.SetPetAsync((uint)temp2 % 10));
                    } else {
                        _logger.LogInformation($"- {info.PlayerName} is a crewmate.");
                        Task.Run(async () => await player.Character.SetSkinAsync((uint)(temp % 15 == 5 ? temp + 1 : temp) % 15));
                        Task.Run(async () => await player.Character.SetHatAsync((uint)(temp2 % 93 == 82 ? temp2 + 1 : temp2) % 93));
                        Task.Run(async () => await player.Character.SetColorAsync((byte)(temp % 11)));
                        Task.Run(async () => await player.Character.SetPetAsync((uint)temp2 % 10));
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
                _games[e.Game.Code] = game;
                _logger.LogInformation($"Countdown started.");
                if (game.Enabled) {
                    foreach (var player in e.Game.Players) {
                        if (player.Client.Id == game.DetectiveClientId)
                            Task.Run(async () => await player.Character.SendChatToPlayerAsync("You are the detective", player.Character));
                        Task.Run(async () => await MakePlayerLookAtChat(player).ConfigureAwait(false));
                    }
                }
            }
        }

        private async Task MakePlayerLookAtChat(IClientPlayer player) {
            await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
            string playername = player.Character.PlayerInfo.PlayerName;
            await player.Character.SetNameAsync($"OPEN CHAT").ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
            await player.Character.SetNameAsync(playername).ConfigureAwait(false);
        }

        [EventListener]
        public void OnGameStarting(IGameStartingEvent e) {
            _logger.LogInformation($"Game is starting.");
            
        }

        [EventListener]
        public void OnPlayerMurder(IPlayerMurderEvent e) {
            _logger.LogInformation($"Player murdered");
            DetectiveGame game = _games[e.Game.Code];
            game.Kills.Add(e.Victim, e.PlayerControl);
            _games[e.Game.Code] = game;
        }

        [EventListener]
        public async void OnPlayerStartMeeting(IPlayerStartMeetingEvent e) {
            _logger.LogInformation($"Player Started Meeting");
            DetectiveGame game = _games[e.Game.Code];
            if (e.Body != null && game.Enabled) {
                _logger.LogInformation($"Body is not null: {e.Body.PlayerInfo.PlayerName}");
                IInnerPlayerControl bMurderer = game.Kills[e.Body];
                _logger.LogInformation($"Found murderer: {bMurderer.PlayerInfo.PlayerName}");

                _logger.LogInformation($"Detective: {game.DetectiveClientId}");
                if (e.ClientPlayer.Client.Id == game.DetectiveClientId) {
                    _logger.LogInformation($"Detective found body: {e.PlayerControl.PlayerInfo.PlayerName}");
                    // Detective found body
                    switch (new Random().Next(1,4)) {
                        case 1:
                            await e.PlayerControl.SendChatToPlayerAsync($"Impostor was wearing {getSkin(bMurderer.PlayerInfo.SkinId)} on their body.");
                            break;
                        case 2:
                            await e.PlayerControl.SendChatToPlayerAsync($"Impostor was wearing {getHat(bMurderer.PlayerInfo.HatId)} on their head.");
                            break;
                        case 3:
                            await e.PlayerControl.SendChatToPlayerAsync($"Impostor was the color {getColor(bMurderer.PlayerInfo.ColorId)}.");
                            break;
                        case 4:
                            await e.PlayerControl.SendChatToPlayerAsync($"Impostor has {getPet(bMurderer.PlayerInfo.PetId)} for a pet.");
                            break;
                        default:
                            await e.PlayerControl.SendChatToPlayerAsync($"Impostor was unable to be detected");
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
