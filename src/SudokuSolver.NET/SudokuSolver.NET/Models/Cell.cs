namespace SudokuSolver.NET.Models;

internal sealed class Cell(int id, string name, int value, bool[] candidates)
{
    public int Id { get; } = id;
    public int Value { get; private set; } = value;

    private readonly List<bool> _originalCandidates = candidates.ToList();
    private readonly List<bool> _candidates = candidates.ToList();
    public IReadOnlyList<bool> Candidates => _candidates.AsReadOnly();
    public string Name { get; } = name;

    public override string ToString()
    {
        return $"{Name}, {Value}, {string.Join("/", Candidates)}";
    }

    public int CurrentCandidateCount()
    {
        return Candidates.Count(x => x);
    }

    public void SetValue(int value)
    {
        Value = value;
        for (var i = 0; i < _candidates.Count; i++)
        {
            _candidates[i] = false;
        }
    }

    public void RemoveCandidate(int value)
    {
        if (_candidates[value])
        {
            _candidates[value] = false;
        }
    }

    public void Reset()
    {
        _candidates.Clear();
        _candidates.AddRange(_originalCandidates);
    }
}