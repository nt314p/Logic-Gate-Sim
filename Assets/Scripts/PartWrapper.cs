public class PartWrapper {

    private Wire top, right;
    private Part node;

    public PartWrapper (Wire top, Wire right, Part node) {
        this.top = top;
        this.right = right;
        this.node = node;
    }

    public PartWrapper () {
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

    public Part GetNode () {
        return node;
    }

    public Wire[] GetWires () {
        return new Wire[] { top, right };
    }

    public Part[] GetParts () {
        return new Part[] { top, right, node };
    }

    public void SetTop (Wire top) {
        this.top = top;
    }

    public void SetRight (Wire right) {
        this.right = right;
    }

    public void SetNode (Part node) {
        this.node = node;
    }

}