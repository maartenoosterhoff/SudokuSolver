namespace SudokuSolver.NET.Models;

internal sealed class Cell
{
    private readonly int _originalValue;

    private readonly List<bool> _originalCandidates;
    private readonly List<bool> _candidates;

    public Cell(int id, string name, int value, bool[] candidates)
    {
        Id = id;
        Value = value;
        _candidates = candidates.ToList();
        Name = name;

        // Readonly wrapper for external usage.
        Candidates = _candidates.AsReadOnly();

        // Backup values, used when resetting
        _originalValue = value;
        _originalCandidates = candidates.ToList();
    }

    public int Id { get; }
    public int Value { get; private set; }
    public string Name { get; }

    public IReadOnlyList<bool> Candidates { get; }

    public override string ToString()
    {
        return $"{Name}, {Value}, {string.Join("/", Candidates)}";
    }

    public int CurrentCandidateCount()
    {
        return Candidates.Count(x => x);
    }

    public int[] GetCurrentCandidates()
    {
        return [.. _candidates.Index().Where(x => x.Item).Select(x => x.Index)];
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
        Value = _originalValue;
        _candidates.Clear();
        _candidates.AddRange(_originalCandidates);
    }
}