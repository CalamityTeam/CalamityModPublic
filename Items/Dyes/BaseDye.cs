using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public abstract class BaseDye : ModItem
    {
        public abstract ArmorShaderData ShaderDataToBind
        {
            get;
        }
        public sealed override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GameShaders.Armor.BindShader(Item.type, ShaderDataToBind);
            }
            SafeSetStaticDefaults();
        }
        public sealed override void SetDefaults()
        {
            byte dye = Item.dye;
            Item.CloneDefaults(ItemID.GelDye);
            Item.dye = dye;
            SafeSetDefaults();
        }
        /// <summary>
        /// Similar to SetDefaults, but always accounts for defining dye IDs
        /// </summary>
        public virtual void SafeSetDefaults()
        {
        }
        /// <summary>
        /// Similar to SetStaticDefaults, but always accounts for shader binding.
        /// </summary>
        public virtual void SafeSetStaticDefaults()
        {
        }
    }
}
