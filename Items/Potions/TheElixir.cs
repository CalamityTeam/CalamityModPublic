using CalamityMod.CalPlayer;
using CalamityMod.Packets;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class TheElixir : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(159, 67, 199),
                new Color(176, 147, 243),
                new Color(84, 50, 185)
            };
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(28, 51, 0, 0, true);
            Item.value = Item.buyPrice(silver: 10); // Sold by Shady Salesman
            Item.rare = ItemRarityID.Blue;
        }

        // Player is unable to use the item when Chaos State is enabled to prevent spamming the item.
        // Even though the item also inflicts Cursed, this is kept as a failsafe in the event the player has something equipped that 
        // provides immunity to Cursed. (i.e Countercurse Mantra or Nazar)
        public override bool CanUseItem(Player player)
        {
            if (player.HasBuff(BuffID.ChaosState))
                return false;

            return base.CanUseItem(player);
        }

        public override bool? UseItem(Player player)
        {
            // 25% chance to "fail" a teleport and send the player to one of three random dangerous locations.
            if (Main.rand.NextBool(4))
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    player.GetModPlayer<TheElixirPlayer>().RunDangerousLocationTeleportation();
                }
                else if (player.whoAmI == Main.myPlayer)
                {
                    TheElixirTeleportSyncPacket.Send(player);
                }

                if (player.whoAmI == Main.myPlayer)
                {
                    player.AddBuff(BuffID.ChaosState, 300, false);
                    player.AddBuff(BuffID.Cursed, 300, false);
                }
            }
            //If it doesn't fail, just act like a Potion of Return
            else
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    player.DoPotionOfReturnTeleportationAndSetTheComebackPoint();
                    SoundEngine.PlaySound(SoundID.Item6);
                }
            }

            //Dust and Teleport sounds happen regardless of if it fails or not
            Rectangle rect = player.getRect();
            int dustAmt = rect.Width * rect.Height / 5;
            for (int k = 0; k < dustAmt; k++)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.TeleportationPotion);
                dust.scale = Main.rand.NextFloat(0.2f, 0.7f);
                if (k < 10)
                    dust.scale += 0.25f;
                if (k < 5)
                    dust.scale += 0.25f;
            }
            for (int k = 0; k < 70; k++)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.DungeonSpirit);
                dust.noGravity = true;
                for (int i = 0; i < 5; i++)
                {
                    if (Main.rand.NextBool(3))
                        dust.velocity *= 0.75f;
                }
                if (Main.rand.NextBool(3))
                {
                    dust.velocity *= 2f;
                    dust.scale *= 1.2f;
                }
                if (Main.rand.NextBool(3))
                {
                    dust.velocity *= 2f;
                    dust.scale *= 1.2f;
                }
                if (Main.rand.NextBool())
                {
                    dust.fadeIn = Main.rand.NextFloat(0.75f, 1f);
                    dust.scale = Main.rand.NextFloat(0.25f, 0.75f);
                }
                dust.scale *= 0.8f;
            }
            return true;
        }
    }

    public class TheElixirPlayer : ModPlayer
    {
        public void RunDangerousLocationTeleportation()
        {
            int locationIndex = Main.rand.Next(3);
            Vector2? location = null;
            if (locationIndex == 0)
                location = CalamityPlayer.GetDungeonArchivesTeleportPosition(Player);
            if (locationIndex == 1)
                location = CalamityPlayer.GetAbyssVoidTeleportPosition(Player);
            if (locationIndex == 2)
                location = CalamityPlayer.GetTempleTeleportPosition(Player);

            if (location.HasValue)
                CalamityPlayer.ModTeleport(Player, location.Value, true, TeleportationStyleID.RecallPotion);
        }
    }
}
