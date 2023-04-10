
# Uri拼接规则

所有的Uri拼接都是通过Uri(Uri baseUri, Uri relativeUri)这个构造器生成。

## 带`/`结尾的baseUri

* `http://a.com/` + `b/c/d` = `http://a.com/b/c/d`
* `http://a.com/path1/` + `b/c/d` = `http://a.com/path1/b/c/d`
* `http://a.com/path1/path2/` + `b/c/d` = `http://a.com/path1/path2/b/c/d`

## 不带`/`结尾的baseUri

* `http://a.com` + `b/c/d` = `http://a.com/b/c/d`
* `http://a.com/path1` + `b/c/d` = `http://a.com/b/c/d`
* `http://a.com/path1/path2` + `b/c/d` = `http://a.com/path1/b/c/d`

事实上`http://a.com`与`http://a.com/`是完全一样的，他们的path都是`/`，所以才会表现一样。为了避免低级错误的出现，请使用的标准baseUri书写方式，即使用`/`作为baseUri的结尾的第一种方式。
