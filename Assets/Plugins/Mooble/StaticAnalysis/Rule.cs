using System.Collections.Generic;
using Mooble.StaticAnalysis.Violation;

namespace Mooble.StaticAnalysis {
  public abstract class Rule<T> {
    public readonly string Name;
    public readonly ViolationLevel Level;

    protected Rule(string name, ViolationLevel level) {
      this.Name = name;
      this.Level = level;
    }

    public abstract List<IViolation> Handle(T thing);
  }
}
