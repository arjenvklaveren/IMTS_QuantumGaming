namespace Game.Data
{
    public enum OpticComponentType
    {
        Mirror = 0,
        BeamSplitter = 1,
        PhaseShifter = 2,
        Test = 3,
        SourceSingle = 4,
        Detector = 5,
        SourceLaser = 6,
        SourcePair = 7,

        // IC Components
        IC1x1 = 100,
        IC2x2 = 101,
        IC2x4 = 102,
        IC4x2 = 103,
        IC4x4 = 104,
        IC2x8 = 105,
        IC8x2 = 106,
        IC4x8 = 107,
        IC8x4 = 108,

        ICIn = 200,
        ICOut = 201,
        ICPhaseShifter = 202,
        ICBeamSplitter = 203,
        WaveGuideStraight = 204,
        WaveGuideCorner = 205,
    }
}
