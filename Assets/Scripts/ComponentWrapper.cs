public class ComponenetWrapper {

    private Wire top, right;
    private Component node;

    public ComponenetWrapper (Wire top, Wire right, Component node) {
        this.top = top;
        this.right = right;
        this.node = node;
    }

    public ComponenetWrapper () {
        this.top = null;
        this.right = null;
        this.node = null;
    }

    public Wire GetTop () {
        return top;
    }

    public Wire GetRight () {
        return right;
    }

    public Component GetNode () {
        return node;
    }

    public Wire[] GetWires () {
        return new Wire[] { top, right };
    }

    public void SetTop (Wire top) {
        this.top = top;
    }

    public void SetRight (Wire right) {
        this.right = right;
    }

    public void SetNode (Component node) {
        this.node = node;
    }

}