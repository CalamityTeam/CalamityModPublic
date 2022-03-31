using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static CalamityMod.CalPlayer.CalamityPlayer;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class DemonshadeHelm : ModItem, IExtendedHat
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demonshade Helm");
            Tooltip.SetDefault("30% increased damage and 15% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.defense = 50;
            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DemonshadeBreastplate>() && legs.type == ModContent.ItemType<DemonshadeGreaves>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            string hotkey = CalamityMod.TarraHotKey.TooltipHotkeyString();
            player.setBonus = "100% increased minion damage and +10 max minions\n" +
                "All attacks inflict the demon flame debuff\n" +
                "Shadowbeams and demon scythes will fire down when you are hit\n" +
                "A friendly red devil follows you around\n" +
                "Press " + hotkey + " to enrage nearby enemies with a dark magic spell for 10 seconds\n" +
                "This makes them do 25% more damage but they also take 125% more damage";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dsSetBonus = true;
            modPlayer.wearingRogueArmor = true;
            modPlayer.WearingPostMLSummonerSet = true;
            if (player.whoAmI == Main.myPlayer && !modPlayer.chibii)
            {
                modPlayer.redDevil = true;
                if (player.FindBuffIndex(ModContent.BuffType<DemonshadeSetDevilBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<DemonshadeSetDevilBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<DemonshadeRedDevil>()] < 1)
                {
                    int damage = (int)(10000 * player.AverageDamage());
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<DemonshadeRedDevil>(), damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }
            player.minionDamage += 1f;
            player.maxMinions += 10;
        }

        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.3f;
            player.Calamity().AllCritBoost(15);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 12);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public void DrawExtension(PlayerDrawInfo drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            int dyeShader = drawPlayer.dye?[0].dye ?? 0;

            Vector2 origin = drawInfo.headOrigin;
            Vector2 headDrawPosition = drawInfo.position.Floor() + origin - Main.screenPosition;

            if (drawPlayer.mount.Active)
                headDrawPosition.Y += drawPlayer.mount.HeightBoost;

            headDrawPosition.X -= drawPlayer.direction == 1f ? 10f : 10f;
            headDrawPosition.Y += drawPlayer.gfxOffY - 14f;

            Texture2D extraPieceTexture = ModContent.GetTexture("CalamityMod/Items/Armor/DemonshadeHelm_Extension");
            Rectangle frame = extraPieceTexture.Frame(1, 20, 0, drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height);
            DrawData pieceDrawData = new DrawData(extraPieceTexture, headDrawPosition, frame, drawInfo.upperArmorColor, drawPlayer.fullRotation, origin, 1f, drawInfo.spriteEffects, 0);
            pieceDrawData.shader = dyeShader;
            Main.playerDrawData.Add(pieceDrawData);
        }
    }
}
