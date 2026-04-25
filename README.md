# Tubes2_OneStack
Tugas Besar 2 IF2211 Strategi Algoritma 2026

## Deskripsi Tugas

Program ini merupakan aplikasi berbasis **ASP.NET Core MVC** yang dirancang untuk melakukan **traversal pohon HTML (DOM Tree)** menggunakan dua algoritma pencarian, yaitu **Breadth First Search (BFS)** dan **Depth First Search (DFS)**.  

Aplikasi menerima masukan dokumen HTML melalui tiga metode:

- Input URL website
- Upload file HTML
- Input kode HTML secara langsung

Dokumen HTML tersebut kemudian akan:

1. di-*scrape* atau dibaca,
2. di-*tokenize* menjadi token HTML,
3. di-*parse* menjadi struktur **DOM Tree**,
4. lalu dilakukan pencarian elemen berdasarkan **CSS Selector**.

Program juga memvisualisasikan:

- struktur DOM Tree hasil parsing,
- node-node yang dilalui selama traversal,
- node-node yang berhasil dipilih oleh CSS Selector.

---

## Penjelasan Singkat Algoritma BFS dan DFS yang Diimplementasikan

### 1. Depth First Search (DFS)

Depth First Search adalah algoritma traversal graf/pohon yang menelusuri node sedalam mungkin pada satu cabang terlebih dahulu sebelum kembali (*backtrack*) ke cabang sebelumnya.

Pada program ini, DFS digunakan untuk:

- menelusuri seluruh node DOM Tree secara rekursif,
- mencari node yang sesuai dengan selector,
- merekam urutan node yang dikunjungi selama proses pencarian.

**Karakteristik DFS pada DOM Tree:**

```text
Root → Child pertama → Child terdalam → kembali → sibling berikutnya
```

DFS cocok untuk eksplorasi mendalam pada struktur HTML yang bertingkat.

---

### 2. Breadth First Search (BFS)

Breadth First Search adalah algoritma traversal yang menelusuri node berdasarkan level/kedalaman.

Traversal dilakukan menggunakan **Queue**, sehingga seluruh node pada level yang sama dikunjungi terlebih dahulu sebelum turun ke level berikutnya.

Pada program ini, BFS digunakan untuk:

- menelusuri node DOM Tree per level,
- mencari node yang cocok dengan CSS selector,
- membandingkan urutan traversal dengan DFS.

**Karakteristik BFS pada DOM Tree:**

```text
Root → seluruh child root → seluruh child level berikutnya → dst
```

BFS cocok untuk pencarian yang mengutamakan node-node terdekat dari akar.

---

## Requirement Program

Sebelum menjalankan program, pastikan perangkat memiliki:

### Software Requirement

- [.NET SDK 10.0](https://dotnet.microsoft.com/)
- Visual Studio Code / Visual Studio 2022
- Browser modern (Chrome / Edge / Firefox)

### Dependency Library

Program menggunakan package:

```bash
Newtonsoft.Json
```

untuk serialisasi DOM Tree menjadi JSON.

---

## Instalasi Dependency

Jika package belum terpasang, jalankan command berikut pada terminal project:

```bash
dotnet add package Newtonsoft.Json --version 13.0.4
```

Lalu lakukan restore package:

```bash
dotnet restore
```

---

## Langkah Compile / Build Program

### 1. Clone Repository

```bash
git clone https://github.com/NafisKreatif/Tubes2_OneStack.git
```

### 2. Masuk ke Folder Project

```bash
cd DOMTreeTraversal
```

### 3. Restore Package

```bash
dotnet restore
```

### 4. Build Program

```bash
dotnet build
```

### 5. Jalankan Program

```bash
dotnet run
```

### Dengan Makefile (opsional)
- Run:
  - `make run`
- Build:
  - `make build`
- Clean:
  - `make clean`

Setelah program berjalan, buka browser dan akses:

```text
http://localhost:5129
```

atau URL localhost yang muncul pada terminal.

---

## Tata Cara Penggunaan Program

### Langkah 1 — Input Dokumen HTML

Pengguna dapat memilih salah satu metode input:

- **Input Link Website**  
  Masukkan URL website yang ingin diambil HTML-nya.

- **Upload File HTML**  
  Upload file dengan ekstensi `.html`.

- **Input HTML Text**  
  Tempelkan source code HTML secara langsung.

---

### Langkah 2 — Generate DOM Tree

Tekan tombol:

```text
Parse HTML into Tree
```

Program akan:

- membaca HTML,
- melakukan tokenisasi,
- membangun struktur DOM Tree,
- menampilkan visualisasi DOM Tree.

---

### Langkah 3 — Input CSS Selector

Masukkan CSS selector yang ingin dicari beserta jumlah maksimalnya, misalnya:

```css
.text
div p
p + span
#container > p
```

---

### Langkah 4 — Pilih Algoritma Traversal

Pilih salah satu:

- DFS
- BFS

---

### Langkah 5 — Jalankan Pencarian

Tekan tombol:

```text
Search
```

Program akan menampilkan:

- node yang dikunjungi saat traversal,
- node yang terpilih sesuai CSS selector,

---

## Tabel Pengerjaan
   <table border = "1">
    <tr>
        <th>No</th>
        <th>Poin</th>
        <th>Ya</th>
        <th>Tidak</th>
    </tr>
    <tr>
        <td>1</td>
        <td>Aplikasi berhasil di kompilasi tanpa kesalahan</td>
        <td>V</td>
        <td></td>
    </tr>
    <tr>
        <td>2</td>
        <td>Aplikasi berhasil dijalankan</td>
        <td>V</td>
        <td></td>
    </tr>
    <tr>
        <td>3</td>
        <td>Aplikasi dapat menerima input URL web, pilihan algoritma, CSS selector, dan jumlah hasil</td>
        <td>V</td>
        <td></td>
    </tr>
    <tr>
        <td>4</td>
        <td>Aplikasi dapat melakukan scraping terhadap web pada input</td>
        <td>V</td>
        <td></td>
    </tr>
    <tr>
        <td>5</td>
        <td>Aplikasi dapat menampilkan visualisasi pohon DOM</td>
        <td>V</td>
        <td></td>
    </tr>
    <tr>
        <td>6</td>
        <td>Aplikasi dapat menelusuri pohon DOM dan menampilkan hasil penelusuran</td>
        <td>V</td>
        <td></td>
    </tr>
    <tr>
        <td>7</td>
        <td>Aplikasi dapat menandai jalur tempuh oleh algoritma</td>
        <td>V</td>
        <td></td>
    </tr>
    <tr>
        <td>8</td>
        <td>Aplikasi dapat menyimpan jalur yang ditempuh algoritma dalam traversal log</td>
        <td>V</td>
        <td></td>
    </tr>
    <tr>
        <td>9</td>
        <td>[Bonus] Membuat video</td>
        <td></td>
        <td>X</td>
    </tr>
    <tr>
        <td>10</td>
        <td>[Bonus] Deploy aplikasi</td>
        <td></td>
        <td>X</td>
    </tr>
    <tr>
        <td>11</td>
        <td>[Bonus] Implementasi animasi pada penelusuran pohon</td>
        <td>V</td>
        <td></td>
    </tr>
    <tr>
        <td>12</td>
        <td>[Bonus] Implementasi multithreading</td>
        <td></td>
        <td>X</td>
    </tr>
    <tr>
        <td>13</td>
        <td>[Bonus] Implementasi LCA Binary Lifting</td>
        <td></td>
        <td>X</td>
    </tr>
</table>

---



## Kelompok
   <table border="1">
    <tr>
        <th>No</th>
        <th>Nama</th>
        <th>NIM</th>
        <th>Tugas</th>
    </tr>
    <tr>
        <td>1</td>
        <td>Muhammad Nafis Habibi</td>
        <td>13524018</td>
        <td>Front-end</td>
    </tr>
    <tr>
        <td>2</td>
        <td>Muhammad Jordan Ferimeison</td>
        <td>13524047</td>
        <td>CSS Selector</td>
    </tr>
    <tr>
        <td>3</td>
        <td>Wildan Abdurrahman Ghazali</td>
        <td>13524054</td>
        <td>HTML Parser</td>
    </tr>
</table>
---