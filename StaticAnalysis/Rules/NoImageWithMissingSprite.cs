#if UNITY_EDITOR
using System.Collections.Generic;

using Mooble.StaticAnalysis.Violation;
using UnityEngine.UI;

namespace Mooble.StaticAnalysis.Rules {
  public class NoImageWithMissingSprite : Rule<Image> {
    public NoImageWithMissingSprite(
      ViolationLevel level = ViolationLevel.Warning,
      ViolationScope scope = ViolationScope.Both) : base(level, scope) {
    }

    public override List<IViolation> Handle(object thing) {
      return this.Handle(thing as Image);
    }

    public override List<IViolation> Handle(Image thing) {
      var violations = new List<IViolation>();

      if (thing == null) {
        return violations;
      }

      if (thing.sprite == null && thing.sprite.GetInstanceID() != 0)
      {
        violations.Add(new NoImageWithMissingSpriteViolation(this.Level, thing));
      }

      return violations;
    }
  }
}
#endif
