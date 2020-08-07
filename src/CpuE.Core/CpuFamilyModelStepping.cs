namespace CpuE.Core
{
    public class CpuFamilyModelStepping
    {
        public byte FullFamily { get; set; }

        public byte Family { get; set; }

        public byte ExtendedFamily { get; set; }

        public byte FullModel { get; set; }

        public byte Model { get => (byte)(FullModel & 0x0F); }

        public byte ExtendedModel { get => (byte) ((FullModel >> 4) & 0x0F); }

        public byte Stepping { get; set; }

        public string ModelHex { get => $"{Model.ToString("X")} ({FullModel.ToString("X")})"; }

        public string FamilyHex { get => $"{Family.ToString("X")} ({FullFamily.ToString("X")})"; }

    }
}
