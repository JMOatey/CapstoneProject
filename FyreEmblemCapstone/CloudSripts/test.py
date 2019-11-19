import pymysql

database_host       = "capstone.ce8exeo9ulmj.us-east-1.rds.amazonaws.com"
database_port       = 3306
database_username   = "admin"
database_password   = "capstone"
database_name       = "capstone"


database = pymysql.connect(
    host=database_host, 
    user=database_username,
    password=database_password,
    db=database_name,
    connect_timeout=5,
    cursorclass=pymysql.cursors.DictCursor)

cursor = database.cursor()

# cursor.execute("SHOW DATABASES")
# print(cursor.fetchall())

sql = "SELECT data FROM save WHERE username=%s"
cursor.execute(sql, ("Test", ))
result = cursor.fetchall()

print([res.get('data') for res in result])

database.commit()