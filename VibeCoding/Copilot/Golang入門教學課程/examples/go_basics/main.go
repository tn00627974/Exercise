
// 1) Go 語言基礎（3～7 天）
package main

import (
    "encoding/json"
    "errors"
    "fmt"
    "sort"
    "strconv"
    "strings"
    "time"
)

// struct + JSON tag（為 JSON 做準備）
type User struct {
    ID        int       `json:"id"`
    Name      string    `json:"name"`
    Email     string    `json:"email"`
    CreatedAt time.Time `json:"created_at"`
}

// 自訂錯誤 + errors.Is 範例
var ErrNotFound = errors.New("not found")

// 簡易 in-memory CRUD
type Store struct {
    users map[int]User
}

func NewStore() *Store {
    return &Store{users: make(map[int]User)}
}

// 多回傳值：回傳 User 與 error
func (s *Store) Get(id int) (User, error) {
    u, ok := s.users[id]
    if !ok {
        return User{}, fmt.Errorf("get user id=%d: %w", id, ErrNotFound)
    }
    return u, nil
}

// 指標接收者：會修改 Store 內容
func (s *Store) Create(name, email string) User {
    nextID := len(s.users) + 1
    u := User{
        ID:        nextID,
        Name:      name,
        Email:     email,
        CreatedAt: time.Now(),
    }
    s.users[u.ID] = u
    return u
}

func (s *Store) Delete(id int) error {
    if _, ok := s.users[id]; !ok {
        return fmt.Errorf("delete user id=%d: %w", id, ErrNotFound)
    }
    delete(s.users, id)
    return nil
}

// slice/map、排序、字串處理
func (s *Store) ListSortedNames() []string {
    names := make([]string, 0, len(s.users))
    for _, u := range s.users {
        names = append(names, u.Name)
    }
    sort.Strings(names)
    return names
}

// defer 範例：確保離開函式前會執行
func withDeferDemo() {
    start := time.Now()
    defer func() {
        fmt.Println("withDeferDemo cost:", time.Since(start))
    }()
    time.Sleep(50 * time.Millisecond)
}

func main() {
    // 變數/常數 + 型別
    const appName = "go-basics"
    version := 1
    fmt.Printf("%s v%d\n", appName, version)

    // if/switch/for
    if version >= 1 {
        fmt.Println("version ok")
    }

    switch time.Now().Weekday() {
    case time.Saturday, time.Sunday:
        fmt.Println("weekend")
    default:
        fmt.Println("weekday")
    }

    sum := 0
    for i := 1; i <= 5; i++ {
        sum += i
    }
    fmt.Println("sum 1..5 =", sum)

    // strings / strconv
    raw := "  Alice,BOB,carol  "
    parts := strings.Split(strings.TrimSpace(raw), ",")
    for i := range parts {
        parts[i] = strings.ToLower(strings.TrimSpace(parts[i]))
    }
    fmt.Println("normalized names:", parts)

    n, err := strconv.Atoi("42")
    if err != nil {
        fmt.Println("atoi error:", err)
        return
    }
    fmt.Println("parsed number:", n)

    // in-memory CRUD
    store := NewStore()
    u1 := store.Create("Alice", "alice@example.com")
    _ = store.Create("Bob", "bob@example.com")

    got, err := store.Get(u1.ID)
    if err != nil {
        fmt.Println("get error:", err)
        return
    }
    fmt.Println("got user:", got.Name)

    // errors.Is
    _, err = store.Get(999)
    if err != nil && errors.Is(err, ErrNotFound) {
        fmt.Println("get 999:", "not found (expected)")
    }

    fmt.Println("sorted names:", store.ListSortedNames())

    // JSON encode/decode
    b, err := json.MarshalIndent(got, "", "  ")
    if err != nil {
        fmt.Println("json marshal error:", err)
        return
    }
    fmt.Println("user json:\n" + string(b))

    var decoded User
    if err := json.Unmarshal(b, &decoded); err != nil {
        fmt.Println("json unmarshal error:", err)
        return
    }
    fmt.Println("decoded user email:", decoded.Email)

    // defer
    withDeferDemo()
}