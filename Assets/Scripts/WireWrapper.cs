public class WireWrapper {

    private Wire top, right;

    public WireWrapper(Wire top, Wire right) {
        this.top = top;
        this.right = right;
    }

    public WireWrapper() {
        this.top = null;
        this.right = null;
    }

    public Wire GetTop () {
        return top;
    }

    public Wire GetRight () {
        return right;
    }

    public void SetTop (Wire top) {
        this.top = top;
    }

    public void SetRight (Wire right) {
        this.right = right;
    }

}