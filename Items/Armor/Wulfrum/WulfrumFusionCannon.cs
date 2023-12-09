using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Cooldowns;
using CalamityMod.Items.BaseItems;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Items.Armor.Wulfrum
{
    public class WulfrumFusionCannon : HeldOnlyItem, IHideFrontArm, ILocalizedModType
    {
        // This is a weapon, but not, I DNC
        public new string LocalizationCategory => "Items.Armor.PreHardmode";
        public static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Item/WulfrumProsthesisShoot") { PitchVariance = 0.1f, Volume = 0.4f };
        public override string Texture => "CalamityMod/Items/Armor/Wulfrum/WulfrumFusionCannon";

        public bool noAnimation = false;

        public override void SetDefaults()
        {
            Item.damage = 6;
            Item.ArmorPenetration = 10;
            Item.DamageType = DamageClass.Summon;
            Item.width = 34;
            Item.height = 42;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = ShootSound;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WulfrumFusionBolt>();
            Item.shootSpeed = 18f;
            Item.holdStyle = 16; //Custom hold style

            Item.useTime = 4;
            Item.useAnimation = 10;
            Item.reuseDelay = 17;
            Item.useLimitPerAnimation = 3;

            Item.noUseGraphic = false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var name = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
            name.OverrideColor = Color.Lerp(new Color(194, 255, 67), new Color(112, 244, 244), 0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f));
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;

            if (player.whoAmI == Main.myPlayer)
            {
                if (!WulfrumHat.HasArmorSet(player))
                {
                    Item.type = 0;
                    Item.SetDefaults(0);
                    Item.stack = 0;

                    Main.mouseItem = new Item();
                }
            }

            Item.noUseGraphic = false;
            if (!player.Calamity().cooldowns.TryGetValue(WulfrumBastion.ID, out var cd) || cd.timeLeft > WulfrumHat.BastionCooldown + WulfrumHat.BastionTime - WulfrumHat.BastionBuildTime)
               Item.noUseGraphic = true;

        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.PiOver4 * 0.1f);

            // This weapon is acquired through usage of an armor set bonus and thus counts as armor. This function must be used.
            damage = player.ApplyArmorAccDamageBonusesTo(damage);
        }

        public override bool CanUseItem(Player player)
        {
            return player.Calamity().cooldowns.TryGetValue(WulfrumBastion.ID, out var cd) && cd.timeLeft < WulfrumHat.BastionCooldown + WulfrumHat.BastionTime - WulfrumHat.BastionBuildTime;
        }

        public void SetItemInHand(Player player, Rectangle heldItemFrame)
        {
            //Make the player face where they're aiming.
            if (player.Calamity().mouseWorld.X > player.Center.X)
            {
                player.ChangeDir(1);
            }
            else
            {
                player.ChangeDir(-1);
            }

            //Hide the cannon till its visible on the player
            if (!player.Calamity().cooldowns.TryGetValue(WulfrumBastion.ID, out var cd) || cd.timeLeft > WulfrumHat.BastionCooldown + WulfrumHat.BastionTime - WulfrumHat.BastionBuildTime)
                return;

            if (player.ItemTimeIsZero)
                noAnimation = false;
            if (player.itemAnimation > Item.useAnimation)
                noAnimation = true;

            float animProgress = 1 - player.itemAnimation / (float)player.itemAnimationMax;
            //It beecomes nan if the player loads into a world with the set bonus already active / without shooting any weapons before using it.
            //this is because itemAnimationMax isnt set before the item gets used once.
            if (noAnimation || animProgress is float.NaN)
                animProgress = 1;

            //Default
            Vector2 itemPosition = player.MountedCenter + new Vector2(-2f * player.direction, -1f * player.gravDir);
            float itemRotation = (player.Calamity().mouseWorld - itemPosition).ToRotation();

            //Adjust for animation

            if (animProgress < 0.9f)
                itemPosition -= itemRotation.ToRotationVector2() * (1 - (float)Math.Pow(1 - (0.9f - animProgress) / 0.9f, 4)) * 3f;

            if (animProgress < 0.6f)
                itemRotation += -0.3f * (float)Math.Pow((0.6f - animProgress) / 0.6f, 2) * player.direction * player.gravDir;

            //Shakezzz
            itemPosition += Main.rand.NextVector2Circular(2f, 2f) * (1 - animProgress);
            

            Vector2 itemSize = new Vector2(38, 18);
            Vector2 itemOrigin = new Vector2(-12, 0);
            CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin, true);
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame) => SetItemInHand(player, heldItemFrame);
        public override void UseStyle(Player player, Rectangle heldItemFrame) => SetItemInHand(player, heldItemFrame);
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => false;
    }
}
