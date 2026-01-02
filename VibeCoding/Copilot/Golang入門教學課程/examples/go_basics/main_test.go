package main

import (
	"errors"
	"reflect"
	"testing"
)

func TestStore_Get(t *testing.T) {
	t.Parallel()

	store := NewStore()
	created := store.Create("Alice", "alice@example.com")

	t.Run("found", func(t *testing.T) {
		t.Parallel()

		
		got, err := store.Get(created.ID)
		if err != nil {
			t.Fatalf("expected nil error, got %v", err)
		}
		if got.ID != created.ID {
			t.Fatalf("expected id=%d, got %d", created.ID, got.ID)
		}
		if got.Name != "Alice" {
			t.Fatalf("expected name=Alice, got %q", got.Name)
		}
	})

	t.Run("not found wraps ErrNotFound", func(t *testing.T) {
		t.Parallel()

		_, err := store.Get(999)
		if err == nil {
			t.Fatalf("expected error")
		}
		if !errors.Is(err, ErrNotFound) {
			t.Fatalf("expected errors.Is(err, ErrNotFound)=true, got false (err=%v)", err)
		}
	})
}

func TestStore_Delete(t *testing.T) {
	t.Parallel()

	store := NewStore()
	created := store.Create("Alice", "alice@example.com")

	if err := store.Delete(created.ID); err != nil {
		t.Fatalf("expected nil error, got %v", err)
	}

	_, err := store.Get(created.ID)
	if err == nil {
		t.Fatalf("expected error after delete")
	}
	if !errors.Is(err, ErrNotFound) {
		t.Fatalf("expected ErrNotFound after delete, got %v", err)
	}
}

func TestStore_ListSortedNames(t *testing.T) {
	t.Parallel()

	store := NewStore()
	_ = store.Create("Bob", "bob@example.com")
	_ = store.Create("Alice", "alice@example.com")
	_ = store.Create("Carol", "carol@example.com")

	got := store.ListSortedNames()
	want := []string{"Alice", "Bob", "Carol"}
	if !reflect.DeepEqual(got, want) {
		t.Fatalf("expected %v, got %v", want, got)
	}
}

func BenchmarkStore_ListSortedNames(b *testing.B) {
	store := NewStore()
	_ = store.Create("Bob", "bob@example.com")
	_ = store.Create("Alice", "alice@example.com")
	_ = store.Create("Carol", "carol@example.com")

	b.ResetTimer()
	for i := 0; i < b.N; i++ {
		_ = store.ListSortedNames()
	}
}

func ExampleStore_Get() {
	store := NewStore()
	created := store.Create("Alice", "alice@example.com")
	got, _ := store.Get(created.ID)
	println(got.Name)
	// Output:
	// Alice
}
