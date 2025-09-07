using System.Collections.Generic;
using SomeGame.Behaviors.Abilities.Base;
using SomeGame.Player.Abilities;

namespace SomeGame.Player.Type1
{
    public partial class AbilitySlashRecallType1 : PlayerAbilityBase
    {
        public override bool CanStart(List<AbilityBase> activeAbilities)
        {
            return false;
        }
    }
}