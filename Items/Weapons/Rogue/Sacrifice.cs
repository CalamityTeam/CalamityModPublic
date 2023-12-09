using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Sacrifice : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.width = Item.height = 68;
            Item.useAnimation = Item.useTime = 9;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SacrificeProjectile>();
            Item.shootSpeed = 16f;
            Item.DamageType = RogueDamageClass.Instance;

            Item.rare = ModContent.RarityType<Violet>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }

        public override bool AltFunctionUse(Player player) => player.ownedProjectileCounts[Item.shoot] > 0;

        public override float StealthDamageMultiplier => 1.65f;
        public override float StealthVelocityMultiplier => 1.5f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type != type || !Main.projectile[i].active || Main.projectile[i].owner != player.whoAmI)
                        continue;

                    if (Main.projectile[i].ai[0] != 1f)
                        continue;

                    NPC attachedNPC = Main.npc[(int)Main.projectile[i].ai[1]];
                    Main.projectile[i].ai[0] = 2f;
                    Main.projectile[i].ModProjectile<SacrificeProjectile>().AbleToHealOwner = attachedNPC.type != NPCID.TargetDummy && attachedNPC.type != ModContent.NPCType<SuperDummyNPC>();
                    Main.projectile[i].netUpdate = true;
                }
                //TODO: Add something here to avoid stealth being consumed
                return false;
            }


            position += velocity * 3f;
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(proj))
                Main.projectile[proj].Calamity().stealthStrike = true;
            return false;
        }
    }
}
