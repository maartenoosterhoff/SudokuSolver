using SudokuSolver.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Core.Solvers.Techniques
{
    public class XWingSolvingTechnique : ISolvingTechnique
    {
        public SolveStep Solve(ISudokuBoardProxy proxy)
        {
            var solutions = from candidateValue in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                            let groupsWithoutValue = (from @group in proxy.SudokuBoard.Groups
                                                      select !@group.CellIds.Any(c => proxy.SudokuBoard.Cells[c].Value == candidateValue)
                                                     ).ToArray()
                            from t in Enumerable.Range(2, proxy.SudokuBoard.CellCount)
                            where t < proxy.SudokuBoard.CellCount - 1
                            let solution = SolveInternal(candidateValue, t, 0, new List<int>(), new List<int>(), 0, groupsWithoutValue)
                            where solution != null
                            select solution;

            return solutions.FirstOrDefault();
        }

        private SolveStep SolveInternal(int v, int t, int phase, List<int> listGroupA, List<int> listGroupB, int start, bool[] sourceGroupList)
        {
            return null;
        }
    }
}
