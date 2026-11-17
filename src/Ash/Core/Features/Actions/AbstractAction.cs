using System;

namespace Ash.Core.Features.Actions;

internal abstract class AbstractAction
{
    internal virtual void QueryInput() {
        throw new NotImplementedException();
    }

    internal virtual void Update() {
        throw new NotImplementedException();
    }
}
