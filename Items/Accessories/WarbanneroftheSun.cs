using System;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Balloon)]
    [LegacyName("SamuraiBadge")]
    public class WarbanneroftheSun : ModItem
    {
        internal const float MaxBonus = 0.2f;
        internal const float MaxDistance = 480f;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Warbanner of the Sun");
            Tooltip.SetDefault("Increases melee damage, true melee damage and melee speed the closer you are to enemies\n" +
                "Max boost is 20% increased melee damage, true melee damage and melee speed");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 78;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.warbannerOfTheSun = true;

            float bonus = CalculateBonus(player);
            player.GetAttackSpeed<MeleeDamageClass>() += bonus;
            player.GetDamage<MeleeDamageClass>() += bonus;
            player.GetDamage<TrueMeleeDamageClass>() += bonus;
        }

        private static float CalculateBonus(Player player)
        {
            float bonus = 0f;

            int closestNPC = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.active && !nPC.friendly && (nPC.damage > 0 || nPC.boss) && !nPC.dontTakeDamage)
                {
                    closestNPC = i;
                    break;
                }
            }
            float distance = -1f;
            for (int j = 0; j < Main.maxNPCs; j++)
            {
                NPC nPC = Main.npc[j];
                if (nPC.active && !nPC.friendly && (nPC.damage > 0 || nPC.boss) && !nPC.dontTakeDamage)
                {
                    float distance2 = Math.Abs(nPC.position.X + (float)(nPC.width / 2) - (player.position.X + (float)(player.width / 2))) + Math.Abs(nPC.position.Y + (float)(nPC.height / 2) - (player.position.Y + (float)(player.height / 2)));
                    if (distance == -1f || distance2 < distance)
                    {
                        distance = distance2;
                        closestNPC = j;
                    }
                }
            }

            if (closestNPC != -1)
            {
                NPC actualClosestNPC = Main.npc[closestNPC];

                float generousHitboxWidth = Math.Max(actualClosestNPC.Hitbox.Width / 2f, actualClosestNPC.Hitbox.Height / 2f);
                float hitboxEdgeDist = actualClosestNPC.Distance(player.Center) - generousHitboxWidth;

                if (hitboxEdgeDist < 0)
                    hitboxEdgeDist = 0;

                if (hitboxEdgeDist < MaxDistance)
                {
                    bonus = MathHelper.Lerp(0f, MaxBonus, 1f - (hitboxEdgeDist / MaxDistance));

                    if (bonus > MaxBonus)
                        bonus = MaxBonus;
                }
            }

            return bonus;
        }
    }
}
