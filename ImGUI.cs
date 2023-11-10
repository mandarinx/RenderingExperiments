using System;
using System.Numerics;
using Godot;
using SysVec2 = System.Numerics.Vector2;
using GodVec2 = Godot.Vector2;
using ImGuiNET;
using RenderingExperiments.Editor;
using Vector4 = System.Numerics.Vector4;

public partial class ImGUI : Node {

    private Dev dev;
    
    public override void _Ready() {
        dev = GetParent<Dev>();
    }

    public override void _Process(double delta) {
        dev.blockInput = false;
        ImGui.Begin("Grid");

        var windowSize = ImGui.GetItemRectSize();
        
        if (ImGui.IsWindowHovered()) {
            dev.blockInput = true;
        }
        
        foreach (var field in dev.fields) {
            
            // if type is not known, look for attribute on field to
            // know which custom drawer to use
            
            bool     isReadonly = false;
            object[] attribs    = field.GetCustomAttributes(false);
            
            foreach (var attrib in attribs) {
                Type attribType = attrib.GetType();
                if (attribType == typeof(SpaceAttribute)) {
                    ImGui.Spacing();
                    ImGui.Spacing();
                }

                if (attribType == typeof(ReadOnlyAttribute)) {
                    isReadonly = true;
                }
            }

            var flags = isReadonly
                ? ImGuiInputTextFlags.ReadOnly
                : ImGuiInputTextFlags.None;
            
            if (isReadonly) {
                ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.21f,0.29f,0.4f,1f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.65f,0.65f,0.65f,1f));
            }

            if (field.FieldType == typeof(GodVec2)) {
                GodVec2 vec2    = (GodVec2)field.GetValue(dev.gridState);
                SysVec2 sysVec2 = new SysVec2(vec2.X, vec2.Y);
                if (ImGui.InputFloat2(field.Name, ref sysVec2, "%.5g", flags)) {
                    field.SetValue(dev.gridState, new GodVec2(sysVec2.X, sysVec2.Y));
                }
            }

            if (field.FieldType == typeof(Int32)) {
                Int32 int32    = (Int32)field.GetValue(dev.gridState);
                if (ImGui.InputInt(field.Name, ref int32, 1, 1, flags)) {
                    field.SetValue(dev.gridState, int32);
                }
            }
            
            if (isReadonly) {
                ImGui.PopStyleColor(2);
            }
        }

        // foreach method
        // - look for button attribute
        
        ImGui.End();
    }
}
