redis-cli.exe sadd cbcart_products 65B82472-8DDF-40C5-9860-BCCEB7C9087D F1E92C06-4258-4063-9E38-D0EEDC90FF46 B94FF1CC-23FF-4269-8085-21717A7D9794
redis-cli.exe hmset cbcart_product:65B82472-8DDF-40C5-9860-BCCEB7C9087D Name Iron Cost 23,00 ForBonusPoints false Id 1
redis-cli.exe hmset cbcart_product:F1E92C06-4258-4063-9E38-D0EEDC90FF46 Name Shoes Cost 102,00 ForBonusPoints true Id 2
redis-cli.exe hmset cbcart_product:B94FF1CC-23FF-4269-8085-21717A7D9794 Name Flower Cost 10,50 ForBonusPoints true Id 3
