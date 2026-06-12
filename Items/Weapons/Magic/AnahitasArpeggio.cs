using System.Collections.Generic;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("SirensSong")]
    public class AnahitasArpeggio : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public float RotationOffset;

        public static readonly SoundStyle CapSound = new("CalamityMod/Sounds/Item/HarpLV6");
        public static readonly SoundStyle EndSound = new("CalamityMod/Sounds/Item/HarpEnd");
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/Item/HarpNoteHit");
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Wet, BuffID.Confused];
        }
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 50;
            Item.damage = 60;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.attackSpeedOnlyAffectsWeaponAnimation = true;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.channel = true;
            Item.noMelee = true;
            Item.knockBack = 6.5f;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AnahitasArpeggioNote>();
            Item.shootSpeed = 13f;
        }

        public override bool CanUseItem(Player player) => player.Calamity().arpeggioCooldown <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Max music note check is in Shoot instead of CanUseItem so that the weapon can still be visually played while at the cap
            int musicNoteCap = Main.zenithWorld ? 7 : 6;

            int nonReleasedMusicNotes = 0;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type != Item.shoot || proj.owner != player.whoAmI || proj.ai[1] == 2f)
                    continue;
                nonReleasedMusicNotes++;
            }

            if (nonReleasedMusicNotes >= musicNoteCap)
            {
                if (!Main.zenithWorld)
                    SoundEngine.PlaySound(CapSound with { Volume = 0.8f }, player.Center);
                return false;
            }
            else
            {
                if (nonReleasedMusicNotes <= 0)
                    RotationOffset = Main.rand.NextFloat(0f, MathHelper.TwoPi);

                Projectile note = Projectile.NewProjectileDirect(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, ai2: nonReleasedMusicNotes);
                note.ModProjectile<AnahitasArpeggioNote>()._randomReleaseRotationOffset = RotationOffset;
                return false;
            }
        }

        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X -= 15f * player.direction;
            player.itemLocation.Y += 15f * player.gravDir;
        }

        // Consume much less mana while the maximum number of notes are present
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            int nonReleasedMusicNotes = Main.projectile.Count(proj => proj.type == Item.shoot && Main.myPlayer == proj.owner && proj.active && proj.ai[1] != 2f);
            if (nonReleasedMusicNotes >= 6)
                mult *= 0.25f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = tooltips.FirstOrDefault(x => x.Text.Contains("[GFB]") && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = Lang.SupportGlyphs(this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipNormal"));
                if (Main.zenithWorld)
                    line.OverrideColor = Main.DiscoColor;
            }
        }
    }
}
