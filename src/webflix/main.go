package main

import (
	"database/sql"
	"github.com/astaxie/beego"
	"github.com/astaxie/beego/orm"
	"github.com/mattn/go-oci8"
	_ "webflix/routers"
)

func init() {
	orm.RegisterDriver("oracle")
	db, err := sql.Open("oci8", "scott/tiger")
}

func main() {
	beego.Run()
}
