namespace SudokuSolver.Core.Events
{
    public class CellCandidateRemovedEventArgs
    {
        public CellCandidateRemovedEventArgs(int cellId, int candidate)
        {
            CellId = cellId;
            Candidate = candidate;
        }

        public int CellId { get; }
        public int Candidate { get; }
    }
}
