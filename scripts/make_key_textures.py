"""
1. Specify a font file as `font_file` variable such as NotoSansJP

2. Install pillow
 $ pip3 install pillow

3. Generate textures
 $ cd scripts
 $ python3 make_key_textures.py 
"""

from PIL import Image, ImageDraw, ImageFont

font_file = 'NotoSansJP-Medium.otf'

kanas = ["あ", "い", "う", "え", "お", 
        "か", "き", "く", "け", "こ", 
        "さ", "し", "す", "せ", "そ", 
        "た", "ち", "つ", "て", "と", 
        "な", "に", "ぬ", "ね", "の", 
        "は", "ひ", "ふ", "へ", "ほ", 
        "ま", "み", "む", "め", "も", 
        "や", "（", "ゆ", "）", "よ", 
        "ら", "り", "る", "れ", "ろ", 
        "わ", "を", "ん", "ー", "～", 
        "、", "。", "？", "！", "…"]

kanas_name = ['A', 'I', 'U', 'E', 'O',
    'Ka', 'Ki', 'Ku', 'Ke', 'Ko',
    'Sa', 'Si', 'Su', 'Se', 'So',
    'Ta', 'Ti', 'Tu', 'Te', 'To',
    'Na', 'Ni', 'Nu', 'Ne', 'No',
    'Ha', 'Hi', 'Hu', 'He', 'Ho',
    'Ma', 'Mi', 'Mu', 'Me', 'Mo',
    'Ya', 'LKakko', 'Yu', 'RKakko', 'Yo',
    'Ra', 'Ri', 'Ru', 'Re', 'Ro',
    'Wa', 'Wo', 'N', 'Bar', 'Nami',
    'Ten', 'Maru', 'Hatena', 'Bikkuri', 'Santen']

size = 128
img_size = (size, size)
font_size = int(size * 0.6)
anchor = (size/2, size/2.2)

font = ImageFont.truetype(font_file, font_size)
color = 'black'

for i , kana in enumerate(kanas):
    img = Image.new('RGBA', img_size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.text(anchor , kana, font=font, spacing=0, anchor="mm", fill=color)
    img.save(f'../Unity/Assets/Resources/Key/Textures/{kanas_name[i]}.png')


font_size = int(size * 0.3)
font = ImageFont.truetype(font_file, font_size)
img = Image.new('RGBA', img_size, (0, 0, 0, 0))
draw = ImageDraw.Draw(img)
anchor = (size/2 * 1.23, size/2 * 0.8)
draw.text(anchor , '゛゜', font=font, spacing=0, anchor="mm", fill=color)
anchor = (size/2, size/2 * 0.8)
draw.text(anchor , '小', font=font, spacing=0, anchor="ma", fill=color)
img.save(f'../Unity/Assets/Resources/Key/Textures/Dakuten.png')
