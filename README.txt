V6 - fixes the missing ruler/grid caused by using PanelContainer as the overlay viewport.

The important structural fix:
- RulerPanel is a PanelContainer only for the background.
- RulerViewport inside it is a plain Control.
- TimelinePanel is a PanelContainer only for the background.
- TimelineViewport inside it is a plain Control.

This matters because Container nodes manage their direct Control children. In V5 the overlay
layers (grid, playhead, ruler content) were direct children of PanelContainer, so the container
could override their layout. That is why the ruler/grid vanished or behaved inconsistently.

Now:
PanelContainer -> plain Control viewport -> overlay layers.

This is the correct pattern for absolute-positioned timeline overlays.
