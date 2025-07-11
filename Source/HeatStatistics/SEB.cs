﻿using System.Text;
using Verse;

namespace RWHS;

public class SEB(string prefix = "", string node = "")
{
    private readonly StringBuilder builder = new();
    private string node = node;


    private SEB Node(string s)
    {
        node = s;
        builder.AppendLine($"{prefix}_{s}".Translate());
        return this;
    }

    private SEB equation()
    {
        builder.AppendLine($"  (= {$"{prefix}_Equation_{node}".Translate()} )");
        return this;
    }

    private SEB calculus(params object[] equationValues)
    {
        builder.AppendLine($"  (= {$"{prefix}_Calculus_{node}".Translate(equationValues)} )");
        return this;
    }

    private SEB Value(float value)
    {
        builder.AppendLine($"  {$"{prefix}_Unit_{node}".Translate(value)}");
        return this;
    }

    public SEB ValueNoFormat(float value)
    {
        builder.AppendLine($"{prefix}_Unit_{node}".Translate(value));
        return this;
    }

    public SEB ValueNoFormat(string value)
    {
        builder.AppendLine($"{prefix}_Unit_{node}".Translate(value));
        return this;
    }

    public SEB Simple(string node, float value)
    {
        return Node(node)
            .Value(value);
    }

    public SEB Full(string node, float value, params object[] equationValues)
    {
        return Node(node)
            .equation()
            .calculus(equationValues)
            .Value(value);
    }

    public override string ToString()
    {
        return builder.ToString();
    }
}