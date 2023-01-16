﻿using ImGuiNET;
using ImGuiScene;
using System;
using System.Numerics;

namespace UIDev
{
  class UITest : IPluginUIMock
  {
    public static void Main(string[] args)
    {
      UIBootstrap.Inititalize(new UITest());
    }

    {% if cookiecutter.include_goat == "yes" -%}
    private TextureWrap goatImage;
    {% endif -%}
    private SimpleImGuiScene scene;

    public void Initialize(SimpleImGuiScene scene)
    {
      {%- if cookiecutter.include_comments == "yes" %}
      // scene is a little different from what you have access to in dalamud
      // but it can accomplish the same things, and is really only used for initial setup here
      // eg, to load an image resource for use with ImGui
      {%- endif %}
      {% if cookiecutter.include_goat == "yes" -%}
      this.goatImage = scene.LoadImage(@"goat.png");
      {% endif -%}
      scene.OnBuildUI += Draw;

      this.Visible = true;

      {% if cookiecutter.include_comments == "yes" -%}
      // saving this only so we can kill the test application by closing the window
      // (instead of just by hitting escape)
      {% endif -%}
      this.scene = scene;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    private bool _isDisposed = false;

    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        {% if cookiecutter.include_goat == "yes" -%}
        this.goatImage.Dispose();
        {%- endif %}
        this._isDisposed = true;
      }
    }

    {% if cookiecutter.include_comments == "yes" -%}
    // You COULD go all out here and make your UI generic and work on interfaces etc, and then
    // mock dependencies and conceivably use exactly the same class in this testbed and the actual plugin
    // That is, however, a bit excessive in general - it could easily be done for this sample, but I
    // don't want to imply that is easy or the best way to go usually, so it's not done here either
    {%- endif %}
    private void Draw()
    {
      DrawMainWindow();
      DrawSettingsWindow();

      if (!Visible)
      {
        this.scene.ShouldQuit = true;
      }
    }

    #region Nearly a copy/paste of PluginUI
    private bool visible = false;
    public bool Visible
    {
      get { return this.visible; }
      set { this.visible = value; }
    }

    private bool settingsVisible = false;
    public bool SettingsVisible
    {
      get { return this.settingsVisible; }
      set { this.settingsVisible = value; }
    }

    {% if cookiecutter.include_comments == "yes" -%}
    // this is where you'd have to start mocking objects if you really want to match
    // but for simple UI creation purposes, just hardcoding values works
    {% endif -%}
    private bool fakeConfigBool = true;

    public void DrawMainWindow()
    {
      if (!Visible)
      {
        return;
      }

      ImGui.SetNextWindowSize(new Vector2(375, 330), ImGuiCond.FirstUseEver);
      ImGui.SetNextWindowSizeConstraints(new Vector2(375, 330), new Vector2(float.MaxValue, float.MaxValue));
      if (ImGui.Begin("My Amazing Window", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
      {
        ImGui.Text($"The random config bool is {this.fakeConfigBool}");

        if (ImGui.Button("Show Settings"))
        {
          SettingsVisible = true;
        }
        {%- if cookiecutter.include_goat == "yes" %}

        ImGui.Spacing();

        ImGui.Text("Have a goat:");
        ImGui.Indent(55);
        ImGui.Image(this.goatImage.ImGuiHandle, new Vector2(this.goatImage.Width, this.goatImage.Height));
        ImGui.Unindent(55);
        {%- endif %}
      }
      ImGui.End();
    }

    public void DrawSettingsWindow()
    {
      if (!SettingsVisible)
      {
        return;
      }

      ImGui.SetNextWindowSize(new Vector2(232, 75), ImGuiCond.Always);
      if (ImGui.Begin("A Wonderful Configuration Window", ref this.settingsVisible,
        ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
      {
        if (ImGui.Checkbox("Random Config Bool", ref this.fakeConfigBool))
        {
          {% if cookiecutter.include_comments == "yes" -%}
          // nothing to do in a fake ui!
          {%- endif %}
        }
      }
      ImGui.End();
    }
    #endregion
  }
}
