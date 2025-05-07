public class TileEntityComponent : TileEntityComposite
{
    public TileEntityComponent(Chunk _chunk) : base(_chunk)
    {
    }

    public TileEntityComponent(TileEntityComposite _original) : base(_original)
    {
    }

    public TileEntityComponent(Chunk _chunk, BlockCompositeTileEntity _block) : base(_chunk, _block)
    {
    }
}