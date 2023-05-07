# Maintainer: Kaleidox <kaleidox@comroid.org>

pkgname=winbash
pkgver=0.1
pkgrel=1
pkgdesc="A description of your package"
arch=('any')
url="https://github.com/yourusername/your-project"
license=('MIT')
depends=('mono')
source=("$pkgname-$pkgver.tar.gz::https://github.com/comroid-git/winbash/archive/v$pkgver.tar.gz")

build() {
    cd "$srcdir/$pkgname-$pkgver"
    xbuild YourProject.sln /p:Configuration=Release
}

package() {
    cd "$srcdir/$pkgname-$pkgver"
    install -Dm755 bin/Release/YourProject.exe "$pkgdir/usr/bin/YourProject.exe"
}
