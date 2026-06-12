using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using CalamityMod.Scenes.MusicScenes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class TheConcoction : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";
        public static int healValue = 9999;
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 30;
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] 
            {
                new Color(255, 190, 250),
                new Color(255, 225, 183),
                new Color(246, 34, 79)
            };
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;
            Item.useStyle = 9;
            Item.useTurn = true;
            Item.useAnimation = (Item.useTime = 17);
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.width = 50;
            Item.height = 56;
            Item.potion = true;

            Item.value = Item.buyPrice(gold: 1); // Sold by Shady Salesman
            Item.rare = ItemRarityID.Green;
        }
        public override bool CanUseItem(Player player)
        {
            TheConcoctionPlayer concoctionPlayer = Main.LocalPlayer.GetModPlayer<TheConcoctionPlayer>();
            return concoctionPlayer.swinesWrathCounter == -1;
        }
        public override void OnConsumeItem(Player player)
        {
            player.HealPlayer(TheConcoction.healValue);
            SoundEngine.PlaySound(SoundID.Item3, player.Center);
            TheConcoctionPlayer concoctionPlayer = player.GetModPlayer<TheConcoctionPlayer>();
            concoctionPlayer.swinesWrathCounter = 1200; // Creates a 10 second delay before the buff is visible (triggers at 600)
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TheConcoctionPlayer concoctionPlayer = Main.LocalPlayer.GetModPlayer<TheConcoctionPlayer>();

            if (concoctionPlayer.hoverTimer <= 10 && concoctionPlayer.hoverTimer > 0)
            {
                TooltipLine healLine = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
                if (healLine != null)
                {
                    healLine.Text = this.GetLocalization("EasterEggText").Value;
                }
            }
        }
    }

    public class TheConcoctionPlayer : ModPlayer
    {
        public int swinesWrathCounter = -1;
        public int spamTimer = 0;
        public bool holdingHeal = false;

        public int hoverTimer = 0;
        public bool wasHovering = false;

        public override void PostUpdate()
        {
            if (!holdingHeal && Player.controlQuickHeal && spamTimer >= 0 && Player.InventoryHas(ModContent.ItemType<TheConcoction>()))
            {
                Vector2 vel = -Vector2.UnitY.RotatedByRandom(0.4f);
                Particle chug = new CustomSpark(Player.Center + vel * 35, vel * Main.rand.NextFloat(2, 3), "CalamityMod/Items/Potions/TheConcoction", false, 22, 0.6f, Color.White, Vector2.One, false, false, 0, false, false, noShrink: true, spin: 0.01f * Math.Sign(vel.X));
                GeneralParticleHandler.SpawnParticle(chug);
                SoundStyle tryChug = new("CalamityMod/Sounds/Custom/AbilitySounds/PotionSicknessOver");
                SoundEngine.PlaySound(tryChug with { pitch = Main.rand.NextFloat(0.6f, 0.8f), MaxInstances = -1 }, Player.Center);
                spamTimer += 80;
                if (spamTimer > 230)
                    spamTimer = 230;
                holdingHeal = true;
            }
            if (!Player.controlQuickHeal)
                holdingHeal = false;
            if (spamTimer > 0)
                spamTimer--;
            if (spamTimer >= 180)
            {
                Player.ConsumeItem(ModContent.ItemType<TheConcoction>());
                swinesWrathCounter = 1200;
                spamTimer = -1;
            }

            if (swinesWrathCounter > 0)
            {
                swinesWrathCounter--;

                if (swinesWrathCounter <= 600 && !Player.HasBuff<SwinesWrathBuff>())
                {
                    Player.AddBuff(ModContent.BuffType<SwinesWrathBuff>(), 600);
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/SwinesWrathProc"), Player.Center);
                }
            }

            if (Main.myPlayer == Player.whoAmI)
            {
                // Check if hover over the right item
                bool isHovering = Main.HoverItem?.type == ModContent.ItemType<TheConcoction>();

                if (isHovering && !wasHovering)
                    hoverTimer = 0;

                if (isHovering)
                    hoverTimer++;
                else
                    hoverTimer = 0;

                // Save the state for the next frame
                wasHovering = isHovering;
            }
        }
    }
}
