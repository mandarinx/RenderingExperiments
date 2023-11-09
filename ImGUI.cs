using Godot;
using System;
using ImGuiNET;

public partial class ImGUI : Node {
    
    
    public override void _Process(double delta) {
        ImGui.Begin("ImGuui");
        ImGui.Text("Whaaat?");
        ImGui.End();
    }
}
