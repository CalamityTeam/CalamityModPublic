using CalamityMod.Cooldowns;
using CalamityMod.DataStructures;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Items.Potions
{
    public class BottledPanacea : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            ItemID.Sets.DrinkParticleColors[Type] = new Color[2] {
                new Color(84, 215, 254),
                new Color(35, 101, 192)
            };
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useTime = Item.useAnimation = 17;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool CanUseItem(Player player) => !player.HasCooldown(PanaceaCooldown.ID);

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = Item.useTime;
                SoundEngine.PlaySound(SoundID.Item4, player.Center);

                for (int i = Player.MaxBuffs - 1; i >= 0; i--)
                {
                    int buffType = player.buffType[i];
                    if (CalamityBuffSets.DebuffDataset[buffType] is not null && (CalamityBuffSets.DebuffDataset[buffType].SicknessDebuffScaling > 0 || CalamityBuffSets.DebuffDataset[buffType].ElectricDebuffScaling > 0))
                        player.DelBuff(i);
                }

                for (int i = 0; i < 4; i++)
                {
                    Vector2 plusVel = new Vector2(Main.rand.NextFloat(-0.55f, 0.55f), Main.rand.NextFloat(-6f, 0f));
                    HealingPlus plus = new(Main.rand.NextVector2FromRectangle(player.Hitbox), Main.rand.NextFloat(1.25f, 1.5f), plusVel, Color.SkyBlue, Color.SkyBlue, 20);
                    GeneralParticleHandler.SpawnParticle(plus);
                }
                for (int d = 0; d < 10; d++)
                    Dust.NewDust(player.position, player.width, player.height, DustID.BlueTorch);

                player.AddCooldown(PanaceaCooldown.ID, CalamityUtils.SecondsToFrames(30));
            }
            return true;
        }
    }
}
