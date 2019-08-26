using System;

public class ButtonInputEventArgs: EventArgs
{
    private readonly ButtonInputType type;

    public ButtonInputEventArgs(ButtonInputType type) {
        this.type = type;
    }

    public ButtonInputType Type {
        get { return type; }
    }
}
