using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Hero;
using GameOfRevenge.WebAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAdmin.Pages
{
    public class _UserHeroesViewModel : PageModel
    {
        public List<UserHeroDetails> Data { get; set; }

        public _UserHeroesViewModel(FullPlayerCompleteData fullPlayerData)
        {
            if (fullPlayerData != null)
            {
                Data = ViewHeroes(fullPlayerData);
            }
        }

        public static List<UserHeroDetails> ViewHeroes(FullPlayerCompleteData fullPlayerData)
        {
            var list = new List<UserHeroDetails>();
            var types = Enum.GetValues(typeof(HeroType));
            foreach (HeroType heroType in types)
            {
                if (heroType == HeroType.Unknown) continue;

                UserHeroDetails hero = null;
                if ((fullPlayerData != null) && (fullPlayerData.Heroes != null))
                {
                    hero = fullPlayerData.Heroes.Find(x => (x.HeroType == heroType));
                }
                if (hero == null) hero = new UserHeroDetails() { HeroType = heroType };

                list.Add(hero);
            }

            return list;
        }

        public static async Task<IActionResult> OnGetHeroesViewAsync(int playerId)
        {
//            Console.WriteLine("get heroes ply="+playerId);
            FullPlayerCompleteData fullPlayerData = null;
            var resp = await BaseUserDataManager.GetFullPlayerData(playerId);
            if (resp.IsSuccess && resp.HasData)
            {
                fullPlayerData = new FullPlayerCompleteData(resp.Data);
            }

            return UserModel.NewPartial("_UserHeroesView", new _UserHeroesViewModel(fullPlayerData));
        }

        public static async Task<IActionResult> OnGetEditHeroViewAsync(int playerId, string heroType)
        {
//            Console.WriteLine("get edit hero ply="+playerId+" type="+heroType);
            InputHeroModel model = null;
            if (Enum.TryParse(heroType, out HeroType hero) && (hero != HeroType.Unknown))
            {
                var resp = await BaseUserDataManager.GetFullPlayerData(playerId);
                if (resp.IsSuccess && resp.HasData)
                {
                    var userHero = resp.Data.Heroes.Find(x => (x.HeroType == hero));
                    if (userHero == null) userHero = new UserHeroDetails() { HeroType = hero };
                    model = new InputHeroModel()
                    {
                        PlayerId = playerId,
                        HeroType = heroType,
                        Hero = userHero
                    };
                }
            }

            return UserModel.NewPartial("Forms/_EditHeroView", model);
        }

        public static async Task<IActionResult> OnPostSaveHeroChangesAsync(InputHeroModel inputHero)
        {
            int playerId = inputHero.PlayerId;
            string heroType = inputHero.HeroType;
            string heroValue = inputHero.HeroValues;
//            Console.WriteLine("post save hero ply = " + playerId + " type = " + heroType + " values = " + heroValue);

            if (!Enum.TryParse(heroType, out HeroType hero) || (hero == HeroType.Unknown) ||
                !int.TryParse(heroValue, out int value))
            {
                throw new Exception("Error in form values");
            }

            var a = new UserHeroManager();
            var updResp = await a.SaveHeroPoints(playerId, hero, value);
            if (!updResp.IsSuccess) throw new Exception(updResp.Message);

            return new JsonResult(new { Success = true });
        }
    }
}