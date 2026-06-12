using CalamityMod.Buffs.Summon.Whips;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ForbiddenCirclet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";

        public static float SummonDamageBoost = 0.1f;
        public static float RogueVelocityBoost = 0.15f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SummonDamageBoost.ToPercent(), RogueVelocityBoost.ToPercent());

        // Set Bonus
        public static float SetBonusRogueStealth = 1.05f;
        public static int TagDuration = CalamityUtils.SecondsToFrames(10);
        public static int StormManaCost = 60;
        public static int StormCooldown = 45;
        public static int StormDamage = 60;
        public static float StormKB = 1f;
        public static int EaterSpawnCount = 6;
        public static int EaterSpawnCooldown = 15;
        public static int EaterDamage = 40;

        public static SummonTag summonTag = new SummonTag()
        {
            //These tag damage fields determine the damage of the spawned eater. The resulting number from each is added together
            //So, 0.5 multiplicative and 10 flat would make the damage be 50% of the spawning hit's damage, plus 10 more.
            //If you want entirely flat, set multiplicative to 0. If you want entirely multiplicative, set flat to 0.
            MultiplicativeTagDamage = 0.25f,
            FlatTagDamage = 5,
            AllowsWhipStacking = true,
            TagOnHit = tagOnHit,
            TagModifyHitEffects = SummonTag.BlankTagModifyHit,
            AutoDrawTooltip = false
        };

        public static void tagOnHit(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(5))
            {
                int damage = (int)Main.player[projectile.owner].GetBestClassDamage().ApplyTo(damageDone * summonTag.MultiplicativeTagDamage) + summonTag.FlatTagDamage;
                Projectile.NewProjectile(projectile.GetSource_OnHit(npc), npc.Center, Main.rand.NextVector2Circular(5f, 5f), ModContent.ProjectileType<ForbiddenCircletEater>(), damage, 3, projectile.owner);
            }
        }

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
            summonTag.TagItem = Type;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 1;
            // This item has the same rarity and sell price as Forbidden Mask
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().donorItem = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ItemID.AncientBattleArmorShirt && legs.type == ItemID.AncientBattleArmorPants;

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowLokis = true;
            player.armorEffectDrawOutlinesForbidden = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            int stormMana = (int)(StormManaCost * player.manaCost);
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusRogueStealth.ToStealth(), CalamityUtils.GetArmorSetBonusKey(), stormMana);
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.forbiddenCirclet = true;
            modPlayer.rogueStealthMax += SetBonusRogueStealth;
            modPlayer.wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += SummonDamageBoost;
            player.Calamity().rogueVelocity += RogueVelocityBoost;
        }

        public override void AddRecipes()
        {
            //Same recipe as Forbidden Mask
            CreateRecipe()
                .AddRecipeGroup("AnyAdamantiteBar", 10)
                .AddIngredient(ItemID.AncientBattleArmorMaterial)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
