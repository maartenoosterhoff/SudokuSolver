using System.Collections.Immutable;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class TechniquesBuilder
{
    public static TechniquesBuilder All => new(Technique.All);

    private TechniquesBuilder(Technique techniques)
    {
        _techniques = techniques;
    }

    public TechniquesBuilder Except(Technique technique)
    {
        _techniques &= ~technique;
        return this;
    }

    private Technique _techniques;

    public ImmutableList<ITechnique> Build()
    {
        var techniques = new List<ITechnique>();

        if ((_techniques & Technique.NakedSingle) != 0) techniques.Add(new NakedSingleTechnique());
        if ((_techniques & Technique.HiddenSingle) != 0) techniques.Add(new HiddenSingleTechnique());
        if ((_techniques & Technique.LockedCandidate) != 0) techniques.Add(new LockedCandidateTechnique());
        if ((_techniques & Technique.NakedMultiple) != 0) techniques.Add(new NakedMultipleTechnique());
        if ((_techniques & Technique.HiddenMultiple) != 0) techniques.Add(new HiddenMultipleTechnique());
        if ((_techniques & Technique.AlignedPairExclusion) != 0) techniques.Add(new AlignedPairExclusionTechnique());

        return [.. techniques];
    }
}