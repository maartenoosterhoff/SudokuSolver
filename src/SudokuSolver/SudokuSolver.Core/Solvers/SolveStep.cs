namespace SudokuSolver.Core.Solvers
{
    public class SolveStep
    {
        public SolveStepItem[] Items { get; set; }
    }

    public class SolveStepItem
    {
        public int[] CellIds { get; set; }
        public int Value { get; set; }
        public SolveStepItemType SolveStepType { get; set; }
        public string Explanation { get; set; }
        public string TechniqueName { get; set; }
    }

    public enum SolveStepItemType
    {
        CandidateRemoval,
        CandidateConfirmation
    }
}
