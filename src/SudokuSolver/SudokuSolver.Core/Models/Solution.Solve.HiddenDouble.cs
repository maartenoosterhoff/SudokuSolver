using System.Linq;

namespace SudokuSolver.Core.Models
{
	partial class Solution
	{
		/// <summary>
		/// Tries to solve the sudoku by finding a Hidden Double. This includes triples and higher.
		/// </summary>
		/// <returns>The description of the step in the solution</returns>
		private SolutionStep TryHiddenDouble()
		{
			var solutions = from t in Enumerable.Range(2, Candidate.PossibleCandidateCount)
							where t < Candidate.PossibleCandidateCount - 1
							from i in Enumerable.Range(0, _groupList.Count)
							select TryHiddenDouble_Internal(t, i, 0, new BitLayer(Candidate.PossibleCandidateCount, false), new BitLayer(Dimension, true))
							;
			var solution = solutions.FirstOrDefault(x => x.StepTaken);
			if (solution != null)
			{
				return solution;
			}

			return new SolutionStep(false);
		}

		private SolutionStep TryHiddenDouble_Internal(int t, int groupId, int nextV, BitLayer b, BitLayer p)
		{
			var step = new SolutionStep(false);

			if (t == b.Count()) {
				if (p.Count() == t) {
					for (var v = 0; v < Candidate.PossibleCandidateCount; v++) {
						if (!b.Layer[v]) {
							if (!(CandidateAsBitLayer(v) & p).IsEmpty()) {
								step.StepTaken = true;
								SetCandidateLayerWithBase(v, false, p);
							}
						}
					}
					if (step.StepTaken) {
						var n = string.Empty;
						for (var v = 0; v < Candidate.PossibleCandidateCount; v++)
						{
							if (b.Layer[v]) {
								if (!string.IsNullOrEmpty(n)) { n += "/"; }
								n += Candidate.PrintValue(v);
							}
						}

						step.Description = "Found a Hidden Double " + n + " in group " + _groupList[groupId].Name + " - removing these candidate(s) from cell(s) " + YieldCells(p) + "\r\n";
					}
				}
			}
			else {
				for (var v = nextV; v < Candidate.PossibleCandidateCount && !step.StepTaken; v++) {
					if (!GroupHasNumber(groupId, v)) {
						var a = CandidateAsBitLayer(v) & GroupAsBitLayer(groupId);
						if (a.Count() <= t && (p & a).Count() <= t) {
							b.Layer[v] = true;
							step = TryHiddenDouble_Internal(t, groupId, v + 1, b, p & a);
							b.Layer[v] = false;
						}
					}
				}
			}

			return step;
		}
	}
}
