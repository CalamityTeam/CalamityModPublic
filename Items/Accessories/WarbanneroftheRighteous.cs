using System;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.NPCs;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Balloon)]
    [LegacyName("SamuraiBadge")]
    [LegacyName("WarbanneroftheSun")]
    public class WarbanneroftheRighteous : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        internal const float MaxBonus = 0.3f;
        internal const float MaxDistance = 720f;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 78;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.WarbanneroftheRighteous = true;
            
            modPlayer.warbannerGlow = !hideVisual;

            if (player.ownedProjectileCounts[ProjectileType<WarbannerLight>()] < 1 && !hideVisual && !player.dead)
            {
                Projectile light = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ProjectileType<WarbannerLight>(), 0, 0f, player.whoAmI);
            }

            int maxValue = (int)(MaxBonus * 100);
            float bonus = CalculateBonus(player) - 0.15f;
            float displayBonus = (int)((bonus + 0.15f) * 100); // Should range from 0 to the maxValue

            if (player.Calamity().cooldowns.TryGetValue(WarbanneroftheRighteousBuff.ID, out var cooldown))
                cooldown.timeLeft = maxValue - (int)displayBonus;
            else
                player.AddCooldown(WarbanneroftheRighteousBuff.ID, maxValue);

            player.Calamity().warbannerDamageMult = bonus;

            int targetCount = 0;
            foreach (NPC npc in Main.ActiveNPCs) // More targets gradually reduces the burn damage for ALL targets
            {
                if (npc.Calamity().warbannerBurnMarked)
                    targetCount++;
            }
            for (int index = 0; index < Main.npc.Length; index++)
            {
                NPC nPC = Main.npc[index];
                float generousHitboxWidth = Math.Max(nPC.Hitbox.Width / 2f, nPC.Hitbox.Height / 2f) + 100; // Adds some room so max bonus isnt when you're ON the hitbox
                float intensity = Utils.Remap(Utils.Distance(player.Center, nPC.Center), MaxDistance + generousHitboxWidth, generousHitboxWidth, 1, 3, true); // We have to have this seperate from bonus, otherwise all effected enemies take damage based on the closest enemy

                if (Utils.Distance(nPC.Center, player.Center) < MaxDistance + generousHitboxWidth && (nPC.IsAnEnemy(true, true, false) || nPC.type == ModContent.NPCType<SuperDummyNPC>()) && !nPC.dontTakeDamage)
                {
                    float minDamageMult = 0.10f;
                    int maxTargets = 7;
                    float damageMult = Utils.Remap(targetCount, maxTargets, 1, minDamageMult, 1);

                    // Handles giving the Warbanner Burn effect
                    CalamityGlobalNPC modNPC = nPC.Calamity();
                    if (!modNPC.warbannerBurnMarked)
                    {
                        modNPC.warbannerBurnDirection = Utils.DirectionTo(player.Center, nPC.Center);
                        modNPC.warbannerBurnMarked = true;
                        modNPC.warbannerBurnTimer = 180;
                    }
                    if (modNPC.warbannerBurnMarked)
                    {
                        modNPC.warbannerBurnIntensity = intensity;
                        modNPC.warbannerBurnDirection = Utils.DirectionTo(player.Center, nPC.Center);
                        int burnDamage = (int)player.GetBestClassDamage().ApplyTo(15 * damageMult); // There is up to a 3x multiplier on this damage depending on distance from the enemy
                        modNPC.warbannerBurnDamage = burnDamage;
                        modNPC.warbannerBurnStacks++;
                        modNPC.warbannerBurnTimer = 180;
                        modNPC.warbannerBurnHideEffects = hideVisual;
                    }
                }
            }
        }

        private static float CalculateBonus(Player player)
        {
            float bonus = 0f;

            NPC closestTarget = player.Center.ClosestNPCAt(MaxDistance * 7); // extra range is to account for bonus range from massive targets
            if (closestTarget != null)
            {
                Vector2 pointToCheck = Vector2.Clamp(player.Center, closestTarget.TopLeft, closestTarget.BottomRight);
                bonus = Utils.Remap(Utils.Distance(player.Center, pointToCheck), MaxDistance, 80, 0, MaxBonus, true);
            }
            else
                bonus = 0;

            return bonus;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryCustomScale(
                spriteBatch,
                texture: TextureAssets.Item[Type].Value,
                position,
                frame,
                drawColor,
                itemColor,
                origin,
                scale,
                wantedScale: 0.6f,
                drawOffset: new(0f, -2f)
            );
            return false;
        }
    }
}
