package main

import (
	"github.com/astaxie/beego"
	"github.com/astaxie/beego/orm"
	"github.com/mattn/go-oci8"
	_ "webflix/routers"
)

func init() {

}

func main() {
	beego.Run()
}
