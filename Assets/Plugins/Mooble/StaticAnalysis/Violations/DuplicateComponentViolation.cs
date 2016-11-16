namespace Mooble.StaticAnalysis.Violation {
  public class DuplicateComponentViolation : IViolation {
    private Rules.DuplicateComponent rule;
    private string gameObjectName;
    private string componentName;

    public DuplicateComponentViolation(Rules.DuplicateComponent r, string o, string c) {
      this.rule = r;
      this.gameObjectName = o;
      this.componentName = c;
    }

    public string Format() {
      return this.rule.Level + " " + this.rule.Name + ": more than one " + this.componentName + " on " + this.gameObjectName;
    }
  }
}
