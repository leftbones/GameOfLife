namespace GOL;

static class RNG {
    public static Random Random = new Random(Guid.NewGuid().GetHashCode());

    public static bool Chance(int n) {
        return Random.Next(1, 100) <= n;
    }

    public static bool CoinFlip() {
        return Chance(50);
    }
}