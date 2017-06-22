import random

table = {}
order = 2

enemies_names = ["goomba", "koopa","paratroopa", "cheep",
                 "bullet", "bill", "hammer", "bro", "podoboo", "lakitu",
                 "spiny", "blooper"]


def load(s):
    for i in range(len(s) - order):
        try:
            table[s[i:i + order]]
        except KeyError:
            table[s[i:i + order]] = []
        table[s[i:i + order]] += s[i + order]


def generate(start = None, max_length = 10):
    if start is None:
        s = random.choice(list(table.keys()))
    else:
        s = start
    try:
        while len(s) < max_length:
            s += random.choice(table[s[-order:]])
    except KeyError:
        pass
    return s


if __name__ == "__main__":

    print('order:', order)
    for enemy_name in enemies_names:
        load(enemy_name)
    print(table)
    for i in range(100):
        print(generate())